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
        private byte _handle;
        private ILogger _logger;
        private Client _client;
        private bool _connectionStateObservableCompleted = false;

        private readonly Dictionary<string, string> _metadata;

        private readonly PluginBuildContext[] _pluginCtxs;


        /// <summary>
        /// A byte representing the index of the scene for this peer.
        /// </summary>
        /// <remarks>
        /// The index is used internally by Stormancer to optimize bandwidth consumption. That means that Stormancer clients can connect to only 256 scenes simultaneously.
        /// </remarks>
        public byte Handle { get { return _handle; } }

        /// <summary>
        /// A string representing the unique Id of the scene.
        /// </summary>
        private SceneAddress _address;
        public SceneAddress Address
        {
            get
            {
                return _address;
            }
        }

        public string Id { get { return Address.SceneId; } }

        private ConnectionStateCtx _connectionState = new ConnectionStateCtx(ConnectionState.Disconnected, "");
        public ConnectionStateCtx CurrentConnectionState
        {
            get
            {
                return _connectionState;
            }
        }

        private Subject<ConnectionStateCtx> _sceneConnectionStateObservable = new Subject<ConnectionStateCtx>();
        public Subject<ConnectionStateCtx> SceneConnectionStateObservable
        {
            get => _sceneConnectionStateObservable;
        }

        /// <summary>
        /// A boolean representing whether the scene is connected or not.
        /// </summary>
        public bool Connected
        {
            get
            {
                return _connectionState.State == ConnectionState.Connected;
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

        private Dictionary<ulong, IScenePeer> _connectedPeers;
        public Dictionary<ulong, IScenePeer> ConnectedPeers
        {
            get
            {
                return _connectedPeers;
            }
        }

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

        public IDependencyResolver DependencyResolver { get; private set; }

        internal Scene(IConnection connection, Client client, SceneAddress sceneAddress, string token, Stormancer.Dto.SceneInfosDto dto, StormancerResolver parentDependencyResolver, PluginBuildContext[] pluginCtxs)
        {
            _address = sceneAddress;
            _peer = connection;
            _token = token;
            _client = client;
            _metadata = dto.Metadata;
            _pluginCtxs = pluginCtxs;
            DependencyResolver = new StormancerResolver(parentDependencyResolver);
            _logger = DependencyResolver.Resolve<ILogger>();

            foreach (var route in dto.Routes)
            {
                _remoteRoutesMap.Add(route.Name, new Route(route.Name, route.Handle, route.Metadata));
            }
        }

        public Action<Packet> OnPacketReceived
        { get; }

        public void Initialize()
        {
            _host = new ScenePeer(_peer, _handle, _remoteRoutesMap, this);
            Action<ConnectionStateCtx> onNext = (state) =>
            {
                _connectionState = state;
            };

            Action<Exception> onError = (exception) =>
            {
                _logger.Log(LogLevel.Error, "Scene", "Connection state change failed", exception.Message);
            };
            _sceneConnectionStateObservable.Subscribe(onNext, onError);
            _peer.GetConnectionStateChangedObservable().Subscribe((state) =>
            {
                var sceneState = CurrentConnectionState;
                // We check the connection is disconnecting, and the scene is not already disconnecting or disconnected
                if (state.State == ConnectionState.Disconnecting && sceneState.State != ConnectionState.Disconnecting && sceneState.State != ConnectionState.Disconnected)
                {
                    MainThread.Post(() =>
                    {
                        SetConnectionState(new ConnectionStateCtx(ConnectionState.Disconnecting, state.Reason));
                    });
                }
                // We check the connection is disconnected, and the scene is not already disconnected
                else if (state.State == ConnectionState.Disconnected && sceneState.State != ConnectionState.Disconnected)
                {
                    // We ensure the scene is disconnecting
                    if (sceneState.State != ConnectionState.Disconnecting)
                    {
                        MainThread.Post(() =>
                        {
                            SetConnectionState(new ConnectionStateCtx(ConnectionState.Disconnecting, state.Reason));
                        });
                    }
                    MainThread.Post(() =>
                    {
                        SetConnectionState(new ConnectionStateCtx(ConnectionState.Disconnected, state.Reason));
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
            SetConnectionState(ConnectionState.Disconnected, reason);
        }

        /// <summary>
        /// Registers a route on the local peer.
        /// </summary>
        /// <param name="route">A string containing the name of the route to listen to.</param>
        /// <param name="handler">An action that is executed when the remote peer call the route.</param>
        /// <returns></returns>
        public void AddRoute(string route, Action<Packet<IScenePeer>> handler, Dictionary<string, string> metadata = null)
        {

            Action<Exception> onError = (exception) => 
		    {
				_logger.Log(LogLevel.Error, "Scene", "Error reading message on route '" + route + "'", exception.Message);
		    };

            OnMessage(route, metadata).Subscribe(handler, onError);

        }

        public IObservable<Packet<IScenePeer>> OnMessage(string route, Dictionary<string, string> metadata = default(Dictionary<string, string>))
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

            if (_connectionState.State != ConnectionState.Disconnected)
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
                routeObj = new Route(route, 0,  metadata);
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
                        origin = ConnectedPeers[data.Connection.Id];
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
        public void SendPacket(PeerFilter filter, string route, Action<Stream> writer, PacketPriority priority = PacketPriority.MEDIUM_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED, string channelIdentifier = "")
        {
            if(_client == null)
            {
                _logger.Error("SendPacket failed: Client deleted");
                throw new InvalidOperationException("Client deleted");
            }
            if(_peer == null)
            {
                _logger.Error("SendPacket failed: Peer deleted");
                throw new InvalidOperationException("Peer deleted");
            }
            if (route.Length == 0)
            {
                _logger.Error("SendPacket failed: Tried to send a message on an invalid route");
                throw new ArgumentNullException("no route selected");
            }
            if (writer == null)
            {
                _logger.Error("SendPacket failed: Tried to send message with a null writer");
                throw new ArgumentNullException("no writer given");
            }
            if (_connectionState.State != ConnectionState.Connected)
            {
                _logger.Error("SendPacket failed: Tried to send message without being connected");
                throw new InvalidOperationException("The scene must be connected to perform this operation.");
            }
            Route routeObj;
            if (!_remoteRoutesMap.TryGetValue(route, out routeObj))
            {
                DependencyResolver.Resolve<ILogger>().Error("SendPacket failed: The route '{0}' doesn't exist on the scene.", route);
                throw new ArgumentException("The route " + route + " doesn't exist on the scene.");
            }

            int channelUid = 1;
            if(channelIdentifier.Length == 0)
            {
                channelUid = _peer.DependencyResolver.Resolve<ChannelUidStore>().GetChannelUid($"Scene_{Id}_{route}");
            }
            else
            {
                channelUid = _peer.DependencyResolver.Resolve<ChannelUidStore>().GetChannelUid(channelIdentifier);
            }
            Action<Stream> writer2 = (stream) =>
            {
                using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, true))
                {
                    binaryWriter.Write(_handle);
                    binaryWriter.Write(routeObj.Handle);
                    writer(stream);
                }
            };
            TransformMetadata metadata = new TransformMetadata(this);

            _peer.SendSystem(writer2, channelUid, priority, reliability, metadata);
        }

        public void SendPacket(string routeName, Action<Stream> writer, PacketPriority priority = PacketPriority.MEDIUM_PRIORITY, PacketReliability reliability = PacketReliability.RELIABLE_ORDERED, string channelIdentifier = "")
        {
            SendPacket(PeerFilter.MatchSceneHost(), routeName, writer, priority, reliability, channelIdentifier); 
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
            if(_connectionState.State == ConnectionState.Disconnected)
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
            else if(_connectionState.State == ConnectionState.Disconnecting)
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
            if(_connectionState.State == ConnectionState.Connected)
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
            else if(_connectionState.State == ConnectionState.Connecting)
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
                _connectionStateObservableCompleted = (state == ConnectionState.Disconnected);

                switch (_connectionState.State)
                {
                    case ConnectionState.Connecting:
                        foreach ( var plugin in _pluginCtxs)
                        {
                            plugin.SceneConnecting?.Invoke(this);
                        }
                        break;
                    case ConnectionState.Connected:
                        foreach (var plugin in _pluginCtxs)
                        {
                            plugin.SceneConnected?.Invoke(this);
                        }
                        break;
                    case ConnectionState.Disconnecting:
                        foreach (var plugin in _pluginCtxs)
                        {
                            plugin.SceneDisconnecting?.Invoke(this);
                        }
                        break;
                    case ConnectionState.Disconnected:
                        foreach (var plugin in _pluginCtxs)
                        {
                            plugin.SceneDisconnected?.Invoke(this);
                        }
                        break;
                }

                _sceneConnectionStateObservable.OnNext(_connectionState);
                if(state == ConnectionState.Disconnected)
                {
                    _sceneConnectionStateObservable.OnCompleted();
                }
            }
        }

        internal void CompleteConnectionInitialization(ConnectionResult cr)
        {
            this._handle = cr.SceneHandle;

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
                Route route = new Route();
                foreach(var localRoute in _localRoutesMap)
                {
                    if(localRoute.Value.Handle == routeId)
                    {
                        route = localRoute.Value;
                        break;
                    }
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
            return new P2PScenePeer(this, connection, p2pService, new P2PConnectToSceneMessage());
        }

        public Task<IP2PScenePeer> OpenP2PConnection(string p2pToken)
        {
            return OpenP2PConnection(p2pToken, CancellationToken.None);
        }
    }
}