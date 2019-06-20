using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Stormancer.Core;
using Stormancer.Plugins;
using UniRx;
using Stormancer.Dto;
using Stormancer.Diagnostics;
using System.Threading;
using Stormancer.Networking;
using Stormancer.Networking.Processors;
using Stormancer.Client45;

namespace Stormancer
{
    /// <summary>
    /// Represents a clientside Stormancer scene.
    /// </summary>
    /// <remarks>
    /// Scenes are created by Stormancer clients through the <see cref="Stormancer.Client.GetScene"/> and <see cref="Stormancer.Client.GetPublicScene"/> methods.
    /// </remarks>
    public class Scene : IScene
    {
        private readonly IConnection _peer;
        private string _token;
        private ILogger _logger;
        private Client _client;
        private bool _connectionStateObservableCompleted = false;

        private readonly Dictionary<string, string> _metadata;

        private readonly PluginBuildContext[] _pluginCtxs;

        /// <summary>
        /// A string representing the unique Id of the scene.
        /// </summary>
        public SceneAddress Address { get; }
        
        public string Id { get { return Address.SceneId; } }

        private ConnectionStateCtx _connectionState = new ConnectionStateCtx(Core.ConnectionState.Disconnected, ""); 
        public ConnectionStateCtx ConnectionState => _connectionState;

        public Subject<ConnectionStateCtx> SceneConnectionStateObservable { get; } = new Subject<ConnectionStateCtx>();

        /// <summary>
        /// A boolean representing whether the scene is connected or not.
        /// </summary>
        public bool Connected
        {
            get
            {
                return ConnectionState.State == Core.ConnectionState.Connected;
            }
        }

        private IScenePeer _host;
        public IScenePeer Host
        {
            get
            {
                return _host;
            }
        }

        public Dictionary<string, IP2PScenePeer> ConnectedPeers { get; } = new Dictionary<string, IP2PScenePeer>();

        private Dictionary<string, Route> _localRoutesMap = new Dictionary<string, Route>();
        private Dictionary<string, Route> _remoteRoutesMap = new Dictionary<string, Route>();

        private ConcurrentDictionary<ushort, Action<Packet>> _handlers = new ConcurrentDictionary<ushort, Action<Packet>>();

        public IConnection HostConnection { get { return _peer; } }

        /// <summary>
        /// Returns a list of the routes registered on the local peer.
        /// </summary>
        public Route[] LocalRoutes
        {
            get
            {
                return _localRoutesMap.Values.ToArray();
            }
        }

        /// <summary>
        /// Returns a list of the routes available on the remote peer.
        /// </summary>
        public Route[] RemoteRoutes
        {
            get
            {
                return _remoteRoutesMap.Values.ToArray();
            }
        }

        public Action<IP2PScenePeer> OnPeerConnected { get; set; }

        public IDependencyResolver DependencyResolver { get; private set; }

        internal Scene(IConnection connection, Client client, SceneAddress sceneAddress, string token, Stormancer.Dto.SceneInfosDto dto, StormancerResolver parentDependencyResolver, PluginBuildContext[] pluginCtxs)
        {
            Address = sceneAddress;
            _peer = connection;
            _token = token;
            _client = client;
            _metadata = dto.Metadata;
            _pluginCtxs = pluginCtxs;
            DependencyResolver = new StormancerResolver(parentDependencyResolver);
            _logger = DependencyResolver.Resolve<ILogger>();

            foreach (var route in dto.Routes)
            {
                _remoteRoutesMap.Add(route.Name, new Route(route.Name, route.Handle, MessageOriginFilter.Host, route.Metadata));
            }
        }

        public Action<Packet> OnPacketReceived
        { get; }

        public void Initialize()
        {
            Action<ConnectionStateCtx> onNext = (state) =>
            {
                _connectionState = state;
            };

            Action<Exception> onError = (exception) =>
            {
                _logger.Log(LogLevel.Error, "Scene", "Connection state change failed", exception.Message);
            };
            SceneConnectionStateObservable.Subscribe(onNext, onError);
            _peer.GetConnectionStateChangedObservable().Subscribe((state) =>
            {
                var sceneState = ConnectionState;
                // We check the connection is disconnecting, and the scene is not already disconnecting or disconnected
                if (state.State == Core.ConnectionState.Disconnecting && sceneState.State != Core.ConnectionState.Disconnecting && sceneState.State != Core.ConnectionState.Disconnected)
                {
                    MainThread.Post(() =>
                    {
                        SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Disconnecting, state.Reason));
                    });
                }
                // We check the connection is disconnected, and the scene is not already disconnected
                else if (state.State == Core.ConnectionState.Disconnected && sceneState.State != Core.ConnectionState.Disconnected)
                {
                    // We ensure the scene is disconnecting
                    if (sceneState.State != Core.ConnectionState.Disconnecting)
                    {
                        MainThread.Post(() =>
                        {
                            SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Disconnecting, state.Reason));
                        });
                    }
                    MainThread.Post(() =>
                    {
                        SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Disconnected, state.Reason));
                    });
                }
            });

            foreach (var plugin in _pluginCtxs)
            {
                plugin.RegisterSceneDependencies(this, DependencyResolver);
            }
            foreach (var plugin in _pluginCtxs)
            {
                plugin.SceneCreated(this);
            }
        }

        /// <summary>
        /// Returns metadata informations for the remote scene host.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetHostMetadata(string key)
        {
            string result = null;
            _metadata.TryGetValue(key, out result);
            return result;
        }

        public ConnectionState GetCurrentConnectionState()
        {
            return _connectionState.State;
        }

        public void SetConnectionState(ConnectionStateCtx state)
        {
            SetConnectionState(state.State, state.Reason);
        }

        private void OnConnectionClosed(string reason)
        {
            SetConnectionState(Core.ConnectionState.Disconnected, reason);
        }

        /// <summary>
        /// Registers a route on the local peer.
        /// </summary>
        /// <param name="route">A string containing the name of the route to listen to.</param>
        /// <param name="handler">An action that is executed when the remote peer call the route.</param>
        /// <returns></returns>
        public void AddRoute(string route, Action<Packet<IScenePeer>> handler, MessageOriginFilter filter = MessageOriginFilter.Host, Dictionary<string, string> metadata = null)
        {

            Action<Exception> onError = (exception) => 
		    {
				_logger.Log(LogLevel.Error, "Scene", "Error reading message on route '" + route + "'", exception.Message);
		    };

            OnMessage(route, filter, metadata).Subscribe(handler, onError);

        }

        public IObservable<Packet<IScenePeer>> OnMessage(string route, MessageOriginFilter filter = MessageOriginFilter.Host, Dictionary<string, string> metadata = default(Dictionary<string, string>))
        {

            if(_client == null)
            {
                throw new InvalidOperationException("The client is deleted.");
            }

            if (route[0] == '@')
            {
                DependencyResolver.Resolve<ILogger>().Log(Stormancer.Diagnostics.LogLevel.Error, this.Id, "AddRoute failed: Tried to create a route with the @ character");
                throw new ArgumentException("A route cannot start with the @ character.");
            }
            metadata = new Dictionary<string, string>();

            if (_connectionState.State != Core.ConnectionState.Disconnected)
            {
                DependencyResolver.Resolve<ILogger>().Error("AddRoute failed: Tried to create a route once connected");
                throw new InvalidOperationException("You cannot register handles once the scene is connected.");
            }

            if (_localRoutesMap.Keys.Contains(route))
            {
                throw new InvalidOperationException("A route already exists for this route name.");
            }

            Route routeObj;
            if (!_localRoutesMap.TryGetValue(route, out routeObj))
            {
                DependencyResolver.Resolve<ILogger>().Trace("Created route with id : '{0}'", route);
                routeObj = new Route(route, 0, filter, metadata);
                _localRoutesMap.Add(route, routeObj);
                foreach (var plugin in _pluginCtxs)
                {
                    plugin.RouteCreated?.Invoke(this, routeObj);
                }
            }
            return OnMessage(routeObj);
        }

        private IObservable<Packet<IScenePeer>> OnMessage(Route route)
        {
            // var index = route.Handle;
            var observable = Observable.Create<Packet<IScenePeer>>(observer =>
            {

                Action<Packet> action = (data) =>
                {
                    IScenePeer origin = null;
                    if(data.Connection.Id == Host.Id)
                    {
                        origin = Host;
                    }
                    else
                    {
                        origin = ConnectedPeers[data.Connection.Key];
                    }
                    if(origin != null)
                    {
                        var packet = new Packet<IScenePeer>(Host, data.Stream, data.Metadata);
                        observer.OnNext(packet);
                    }
                    
                };
                route.Handlers += action;

                return UniRx.Disposable.Create(() =>
                {
                    route.Handlers -= action;
                });
            });
            return observable;
        }

        /// <summary>
        /// Sends a packet to the scene.
        /// </summary>
        /// <param name="route">A string containing the route on which the message should be sent.</param>
        /// <param name="writer">An action called.</param>
        /// <returns>A task completing when the transport takes</returns>
        public void Send(PeerFilter filter, string route, Action<Stream> writer, PacketPriority priority = PacketPriority.MEDIUM_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED, string channelIdentifier = "")
        {
            if (_client == null)
            {
                _logger.Error("SendPacket failed: Client deleted");
                throw new InvalidOperationException("Client deleted");
            }
            if (_connectionState.State != Core.ConnectionState.Connected)
            {
                _logger.Error("SendPacket failed: Tried to send message without being connected");
                throw new InvalidOperationException("The scene must be connected to perform this operation.");
            }
            if (route.Length == 0)
            {
                _logger.Error("SendPacket failed: Tried to send a message on an invalid route");
                throw new ArgumentNullException("no route selected");
            }
            if (filter.Type == PeerFilterType.MatchSceneHost)
            {
                Send(_host, route, writer, priority, reliability, channelIdentifier);
            }
            else
            {
                if (filter.Type == PeerFilterType.MatchAllP2P)
                {
                    foreach (var scenePeer in ConnectedPeers)
                    {
                        Send(scenePeer.Value, route, writer, priority, reliability, channelIdentifier);
                    }
                }
                else if (filter.Type == PeerFilterType.MatchPeers)
                {
                    if (filter.Ids.Length == 0)
                    {
                        throw new InvalidOperationException("Cannot Send to peers if there is no peer ids in the PeerFilter");
                    }
                    foreach (string id in filter.Ids)
                    {
                        if (ConnectedPeers.TryGetValue(id, out var peer))
                        {
                            Send(peer, route, writer, priority, reliability, channelIdentifier);
                        }
                        else
                        {
                            throw new InvalidOperationException($"The peer with Id {id} cannot be found in the ConnectedPeers map");
                        }
                    }

                }
            }
        }

        private void Send(IScenePeer scenePeer, string routeName, Action<Stream> writer, PacketPriority priority = PacketPriority.MEDIUM_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED, string channelIdentifier = "")
        {
            var remoteRoutes = scenePeer.Routes;
            if(!remoteRoutes.TryGetValue(routeName, out var route))
            {
                throw new InvalidOperationException($"The scene peer does not contains a route named {routeName}");
            }

            var connection = scenePeer.Connection;
            var channelUidStore = connection.DependencyResolver.Resolve<ChannelUidStore>();
            int channelUid = 1;
            if(string.IsNullOrEmpty(channelIdentifier))
            {
                channelUid = channelUidStore.GetChannelUid($"Scene_{Id}_{routeName}");
            }
            else
            {
                channelUid = channelUidStore.GetChannelUid(channelIdentifier);
            }

            var sceneHandle = scenePeer.Handle;
            var routeHandle = route.Handle;

            Action<Stream> writer2 = stream =>
            {
                using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(sceneHandle);
                    binaryWriter.Write(routeHandle);
                    writer?.Invoke(stream);
                }
                
            };
            var transformMetadata = new TransformMetadata(this);
            connection.SendSystem(writer2, channelUid, priority, reliability, transformMetadata);

        }

        public void Send(string routeName, Action<Stream> writer, PacketPriority priority = PacketPriority.MEDIUM_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED, string channelIdentifier = "")
        {
            Send(PeerFilter.MatchSceneHost(), routeName, writer, priority, reliability, channelIdentifier); 
        }
               
        public Task Connect()
        {
            return Connect(CancellationToken.None);
        }

        /// <summary>
        /// Connects the scene to the server.
        /// </summary>
        /// <returns>A task completed once the connection is complete.</returns>
        /// <remarks>
        /// The task is susceptible to throw an exception in case of connection error.
        /// </remarks>
        public async Task Connect(CancellationToken ct)
        {
            if(_connectionState.State == Core.ConnectionState.Disconnected)
            {
                if(_client != null)
                {
                    await this._client.ConnectToScene(this, this._token, this._localRoutesMap.Values, ct);
                }
                else
                {
                    throw new InvalidOperationException("Client is deleted.");
                }
            }
            else if(_connectionState.State == Core.ConnectionState.Disconnecting)
            {
                throw new InvalidOperationException("Scene is disconnecting");
            }
  
            _logger.Info("Successfully connected to scene : '{0}'.", Id);
        }

        /// <summary>
        /// Disconnects the scene.
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            _logger.Log(LogLevel.Trace, "Scene", "Scene disconnecting");
            if(_connectionState.State == Core.ConnectionState.Connected)
            {
                if(_client != null)
                {
                    await _client.Disconnect(this);
                    
                }
                else
                {
                    _logger.Log(LogLevel.Warn, "Scene", "Client is invalid");
                    throw new InvalidOperationException("Client is Invalid");
                }
            }
            else if(_connectionState.State == Core.ConnectionState.Connecting)
            {
                throw new InvalidOperationException("Client is Invalid");
            }

            DependencyResolver.Resolve<ILogger>().Trace("Client disconnected from the scene "+Id);

        }

        private void SetConnectionState(ConnectionState state, string reason="")
        {
            if(_connectionState.State != state && !_connectionStateObservableCompleted)
            {
                _connectionState = new ConnectionStateCtx(state, reason);
                _connectionStateObservableCompleted = (state == Core.ConnectionState.Disconnected);

                switch (_connectionState.State)
                {
                    case Core.ConnectionState.Connecting:
                        foreach ( var plugin in _pluginCtxs)
                        {
                            plugin.SceneConnecting?.Invoke(this);
                        }
                        break;
                    case Core.ConnectionState.Connected:
                        foreach (var plugin in _pluginCtxs)
                        {
                            plugin.SceneConnected?.Invoke(this);
                        }
                        break;
                    case Core.ConnectionState.Disconnecting:
                        foreach (var plugin in _pluginCtxs)
                        {
                            plugin.SceneDisconnecting?.Invoke(this);
                        }
                        break;
                    case Core.ConnectionState.Disconnected:
                        foreach (var plugin in _pluginCtxs)
                        {
                            plugin.SceneDisconnected?.Invoke(this);
                        }
                        break;
                }

                SceneConnectionStateObservable.OnNext(_connectionState);
                if(state == Core.ConnectionState.Disconnected)
                {
                    SceneConnectionStateObservable.OnCompleted();
                }
            }
        }

        internal void CompleteConnectionInitialization(ConnectionResult cr)
        {
            _host = new ScenePeer(_peer, cr.SceneHandle, _remoteRoutesMap, this);
            foreach (var route in _localRoutesMap)
            {
                route.Value.Handle = cr.RouteMappings[route.Key];
                _handlers.TryAdd(route.Value.Handle, route.Value.Handlers);
            }
        }

        /// <summary>
        /// Fires when packets are received on the scene.
        /// </summary>
        public Action<Packet> PacketReceived;
        
        internal void HandleMessage(Packet packet)
        {
            PacketReceived?.Invoke(packet);

            var temp = new byte[2];
            //Extract the route id.
            packet.Stream.Read(temp, 0, 2);
            var routeId = BitConverter.ToUInt16(temp, 0);

            if(_handlers.ContainsKey(routeId))
            {
                var route = new Route();
                foreach(var localRoute in _localRoutesMap)
                {
                    if(localRoute.Value.Handle == routeId)
                    {
                        route = localRoute.Value;
                        break;
                    }
                }

                if(packet.Connection.Id == _host.Id && (route.Filter & MessageOriginFilter.Host) == 0)
                {
                    return; // The route doesn't accept messages from the scene host.
                }
                if(packet.Connection.Id != _host.Id && (route.Filter & MessageOriginFilter.Host) > 0)
                {
                    return; // The route doesn't accept messages from the scene host.
                }

                packet.Metadata["routeId"] = route.Name;
                foreach (var ctx in _pluginCtxs)
                {
                    ctx.PacketReceived?.Invoke(packet);
                }
                Action<Packet> observer;

                if (_handlers.TryGetValue(routeId, out observer))
                {
                    try
                    {
                        observer(packet);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(Diagnostics.LogLevel.Error, "scene.route", "An exception occurred while trying to process a route message", ex);
                        throw;
                    }
                }
            }            
        }

        /// <summary>
        /// List containing the scene host connection.
        /// </summary>
        public IScenePeer[] RemotePeers
        {
            get
            {
                return new IScenePeer[]{Host};
            }
        }

        public bool IsHost
        {
            get { return false; }
        }

        public Dictionary<string, string> GetSceneMetadata()
        {
            return _metadata;
        }

        public P2PTunnel RegisterP2PServer(string p2pServerId)
        {
            var p2pService = DependencyResolver.Resolve<P2PService>();
            return p2pService.RegisterP2PServer(Id + "." + p2pServerId);
        }

        public async Task<IP2PScenePeer> OpenP2PConnection(string p2pToken, CancellationToken ct)
        {
            var p2pService = DependencyResolver.Resolve<P2PService>();
            var connection = await p2pService.OpenP2PConnection(_peer, p2pToken, ct);
            var handles = connection.DependencyResolver.Resolve<List<Scene>>();
            byte handle = 0;
            bool success = false;

            for(byte i = 0; i < 150; i++)
            {
                if(handles.ElementAtOrDefault(i) == null)
                {

                    handle = (byte)(i + (byte)MessageIDTypes.ID_SCENES);
                    handles.Insert(i, this);
                    success = true;
                    break;
                }
            }
            if(!success)
            {
                throw new InvalidOperationException("Failed to generate handle for scene");
            }

            var connectToSceneMessage = new P2PConnectToSceneMessage();
            connectToSceneMessage.SceneId = Address.toUri();
            connectToSceneMessage.SceneHandle = handle;
            foreach (var r in LocalRoutes)
            {
                if (((byte)r.Filter & (byte)MessageOriginFilter.Peer) >  0)
                {
                    var routeDto = new RouteDto();
                    routeDto.Handle = r.Handle;
                    routeDto.Name = r.Name;
                    routeDto.Metadata = r.Metadata;
                    connectToSceneMessage.Routes.Add(routeDto);
                }
            }
            connectToSceneMessage.ConnectionMetadata = connection.Metadata;
            connectToSceneMessage.SceneMetadata = GetSceneMetadata();

            var message = await SendSystemRequest<P2PConnectToSceneMessage, P2PConnectToSceneMessage>(connection, (byte)SystemRequestIDTypes.ID_CONNECT_TO_SCENE, connectToSceneMessage, ct);
            _logger.Log(LogLevel.Debug, "Debug", "Received response from ID_CONNECT_TO_SCENE, adding the peerConnected");
            var peer = AddConnectedPeer(connection, p2pService, message);
            var requestProcessor = DependencyResolver.Resolve<RequestProcessor>();
            var serializer = DependencyResolver.Resolve<ISerializer>();
            await requestProcessor.SendSystemRequest(connection, (byte)SystemRequestIDTypes.ID_CONNECTED_TO_SCENE, stream => { serializer.Serialize(message.SceneId, stream); });
            SetPeerConnected(connection);
            return peer;
        }

        public void SetPeerConnected(IConnection connection)
        {
            if (ConnectedPeers.TryGetValue(connection.Key, out var peer))
            {
                OnPeerConnected?.Invoke(peer);
            }
        }

        public IP2PScenePeer AddConnectedPeer(IConnection connection, P2PService service, P2PConnectToSceneMessage message)
        {
            if (!ConnectedPeers.TryGetValue(connection.Key, out var peer))
            {
                peer = new P2PScenePeer(this, connection, service, message);
                ConnectedPeers.Add(connection.Key, peer);
            }
            return peer;
        }


        private Task<T1> SendSystemRequest<T1, T2>(IConnection connection, byte id, T2 parameter, CancellationToken token = default(CancellationToken))
        {
            var requestProcessor = DependencyResolver.Resolve<RequestProcessor>();
            return requestProcessor.SendSystemRequest<T1, T2>(connection, id, parameter, token);
        }
        public Task<IP2PScenePeer> OpenP2PConnection(string p2pToken)
        {
            return OpenP2PConnection(p2pToken, CancellationToken.None);
        }
    }
}