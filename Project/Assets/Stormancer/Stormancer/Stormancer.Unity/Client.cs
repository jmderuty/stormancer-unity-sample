using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Stormancer.Client45.Infrastructure;
using Stormancer.Client45;
using Stormancer.Networking;
using Stormancer.Core;
using System.Diagnostics;
using Stormancer.Networking.Processors;
using Stormancer.Diagnostics;
using Stormancer.Processors;
using Stormancer.Plugins;
using System.Text;
using Stormancer.Infrastructure;
using UniRx;

namespace Stormancer
{
    /// <summary>
    /// Stormancer client library
    /// </summary>
    public class Client : IDisposable
    {
        private class ClientScene
        {
            public Task<Scene> Task { get; set; }
            public bool Connecting { get; set; } = false;
            public bool IsPublic { get; set; } = true;
        }

        private readonly ClientConfiguration _config;
        private readonly string _accountId;
        private readonly string _applicationName;
        private readonly int _pingInterval = 5000;
        private TimeSpan _serverTimeout;
        private string _sessionToken;

        private readonly PluginBuildContext _pluginCtx = new PluginBuildContext();
        private IConnection _serverConnection;
        public AddressType AddressType { get; set; } = AddressType.NotYetDefined;

        private IPacketDispatcher _dispatcher;

        private readonly Task _initializationTask = null;

        private readonly ISerializer _systemSerializer;

        private Stormancer.Networking.Processors.RequestProcessor _requestProcessor;
        private Stormancer.Processors.SceneDispatcher _scenesDispatcher;
        private Dictionary<string, ISerializer> _serializers = new Dictionary<string, ISerializer>();
        private ConcurrentDictionary<string, ClientScene> _scenes = new ConcurrentDictionary<string, ClientScene>();

        private CancellationTokenSource _cts;
        private ushort _maxPeers;

        private Dictionary<string, string> _metadata;
        /// <summary>
        /// The name of the Stormancer server application the client is connected to.
        /// </summary>
        public string ApplicationName
        {
            get
            {
                return this._applicationName;
            }
        }

        private readonly IScheduler _scheduler;
        public StormancerResolver DependencyResolver { get; private set; }

        /// <summary>
        /// An user specified logger.
        /// </summary>
        private ILogger _logger;
        public ILogger Logger
        {
            get => _logger;
        }

        /// <summary>
        /// Creates a Stormancer client instance.
        /// </summary>
        /// <param name="configuration">A configuration instance containing options for the client.</param>
        public Client(ClientConfiguration configuration)
        {
            _config = configuration ?? throw new ArgumentNullException("configuration", "A stormancer client confirguration should not be null");

            _serverTimeout = _config.DefaultTimeout;
            _systemSerializer = configuration.Serializers.First(s => s.Name == MsgPackMapSerializer.NAME);
            _cts = new CancellationTokenSource();
            _logger = _config.Logger;
            this._pingInterval = configuration.PingInterval;
            this._scheduler = configuration.Scheduler;
            DependencyResolver = new StormancerResolver();
            DependencyResolver.RegisterDependency<ISerializer>(_systemSerializer);
            DependencyResolver.RegisterDependency<SynchronizationContext>(configuration.SynchronizationContext);
            this.DependencyResolver.Register<ITransport>(configuration.TransportFactory, true);
            this._accountId = configuration.Account;
            this._applicationName = configuration.Application;
            //TODO handle scheduler in the transport
            this._dispatcher = configuration.Dispatcher;
            this._metadata = configuration._metadata;

            foreach (var serializer in configuration.Serializers)
            {
                this._serializers.Add(serializer.Name, serializer);
            }

            this._maxPeers = configuration.MaxPeers;

            foreach (var plugin in configuration.Plugins)
            {
                plugin.Build(_pluginCtx);
            }
            ConfigureContainer();


            var ev = _pluginCtx.ClientCreated;
            if (ev != null)
            {
                ev(this);
            }

            _transport = DependencyResolver.Resolve<ITransport>();
            this._metadata.Add("serializers", string.Join(",", this._serializers.Keys.ToArray()));
            this._metadata.Add("transport", _transport.Name);
            this._metadata.Add("version", "1.1.0");
            this._metadata.Add("platform", "Unity");
            this._metadata.Add("protocol", "2");

            async Task Initialize()
            {
                this._logger.Trace("Client", "creating client");

                _transport.PacketReceived += Transport_PacketReceived;
                _logger.Log(Diagnostics.LogLevel.Trace, "Client", "Starting transport", $"port :{_config.ClientSDKPort}; maxPeers: {_maxPeers + 1}");
                _connections = DependencyResolver.Resolve<IConnectionManager>();
                await _transport.Start("client", _connections, _cts.Token, _config.ClientSDKPort, (ushort)(_maxPeers + 1));

                this._watch.Start();

            }

            _initializationTask = Initialize();
        }

        private Stopwatch _watch = new Stopwatch();

        /// <summary>
        /// Synchronized clock with the server.
        /// </summary>
        public long Clock
        {
            get
            {
                return _watch.ElapsedMilliseconds + _offset;
            }
        }

        /// <summary>
        /// Last ping value with the cluster.
        /// </summary>
        /// <remarks>
        /// 0 means that no mesure has be made yet.
        /// </remarks>
        public long LastPing
        {
            get;
            private set;
        }
        private long _offset;

        private IConnectionManager _connections;

        private void ConfigureContainer()
        {
            DependencyResolver.RegisterDependency<ClientConfiguration>(_config);
            DependencyResolver.Register<ILogger>(() => _config.Logger);
            var tokenHandler = new TokenHandler(_config);
            DependencyResolver.Register<ITokenHandler>(() => tokenHandler);
            DependencyResolver.Register(() => new ApiClient(_config, _logger, tokenHandler));
            DependencyResolver.RegisterDependency<IConnectionHandler>(new IConnectionHandler());
            DependencyResolver.RegisterDependency<IClock>(new IClock(this));
            DependencyResolver.RegisterDependency<RequestProcessor>(new RequestProcessor(_config.Logger, DependencyResolver.Resolve<ISerializer>()));
            DependencyResolver.RegisterDependency<IConnectionManager>(new ConnectionRepository(_config.Logger));

            DependencyResolver.RegisterDependency(new P2PSessions(DependencyResolver.Resolve<IConnectionManager>()));

            DependencyResolver.RegisterDependency(new P2PTunnels(
                DependencyResolver.Resolve<RequestProcessor>(),
                DependencyResolver.Resolve<IConnectionManager>(),
                DependencyResolver.Resolve<ISerializer>(),
                DependencyResolver.Resolve<ClientConfiguration>(),
                DependencyResolver.Resolve<ILogger>()
            ));
            DependencyResolver.RegisterDependency(new P2PService(
                DependencyResolver.Resolve<IConnectionManager>(),
                DependencyResolver.Resolve<RequestProcessor>(),
                DependencyResolver.Resolve<ITransport>(),
                DependencyResolver.Resolve<ISerializer>(),
                DependencyResolver.Resolve<P2PTunnels>(),
                DependencyResolver.Resolve<ILogger>()
            ));

            DependencyResolver.RegisterDependency(new P2PRequestModule(
                DependencyResolver.Resolve<ITransport>(),
                DependencyResolver.Resolve<IConnectionManager>(),
                DependencyResolver.Resolve<P2PSessions>(),
                DependencyResolver.Resolve<ISerializer>(),
                DependencyResolver.Resolve<P2PTunnels>(),
                DependencyResolver.Resolve<ILogger>(),
                DependencyResolver.Resolve<ClientConfiguration>(),
                this
            ));

            DependencyResolver.RegisterDependency(new P2PPacketDispatcher(
                DependencyResolver.Resolve<P2PTunnels>(),
                DependencyResolver.Resolve<IConnectionManager>(),
                DependencyResolver.Resolve<ILogger>(),
                DependencyResolver.Resolve<ISerializer>()
            ));

            _requestProcessor = DependencyResolver.Resolve<RequestProcessor>();

            // Initialize Request processor
            IRequestModule[] modules = new IRequestModule[1];
            modules[0] = DependencyResolver.Resolve<P2PRequestModule>();
            RequestProcessor.Initialize(DependencyResolver.Resolve<RequestProcessor>(), modules);


            _scenesDispatcher = new Processors.SceneDispatcher();
            this._dispatcher.AddProcessor(_requestProcessor);
            this._dispatcher.AddProcessor(DependencyResolver.Resolve<P2PPacketDispatcher>());
            this._dispatcher.AddProcessor(_scenesDispatcher);
            DependencyResolver.RegisterDependency<SceneDispatcher>(_scenesDispatcher);


#if UNITY_EDITOR
            IConnectionHandler temp = DependencyResolver.Resolve<IConnectionHandler>();
            temp.PeerConnected += (PeerConnectedContext pcc) =>
            {
                ConnectionWrapper connection = new ConnectionWrapper(pcc.Connection, _config.Plugins.OfType<StormancerEditorPlugin>().First());
                pcc.Connection = connection;
            };
#endif
        }

        private void Transport_PacketReceived(Stormancer.Core.Packet obj)
        {
            _dispatcher.DispatchPacket(obj, _config.SynchronizationContext);
        }


        /// <summary>
        /// Returns a public scene (accessible without authentication)
        /// </summary>
        /// <remarks>
        /// The effective connection happens when "Connect" is called on the scene.
        /// </remarks>
        /// <param name="sceneId">The id of the scene to connect to.</param>
        /// <param name="userData">User data that should be associated to the connection.</param>
        /// <returns>A task returning the scene</returns>
        public async Task<Scene> GetPublicScene(string sceneId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (sceneId.Length == 0)
            {
                _logger.Log(Diagnostics.LogLevel.Error, "Client", "Empty scene Id");
                throw new ArgumentException("Empty scene id");
            }
            SceneAddress sceneAddress = await ParseSceneUrl(sceneId, cancellationToken);
            var uSceneId = sceneAddress.toUri();
            await _initializationTask;
            _logger.Log(Diagnostics.LogLevel.Trace, "Client", "Get public scene", uSceneId);

            ClientScene container;
            if (_scenes.TryGetValue(uSceneId, out container))
            {
                if (container.IsPublic)
                {
                    return await container.Task;
                }
                else
                {
                    _logger.Log(Diagnostics.LogLevel.Error, "Client", "The scene is private");
                    throw new InvalidOperationException("The scene is private");
                }
            }
            else
            {
                container = new ClientScene();
                container.IsPublic = true;
                var federation = await GetFederation(cancellationToken);
                var endpoint = await DependencyResolver.Resolve<ApiClient>().GetSceneEndpoint(new List<string>(federation.GetCluster(sceneAddress.ClusterId).Endpoints), sceneAddress.Account, sceneAddress.App, sceneAddress.SceneId, cancellationToken);
                Scene scene;
                try
                {
                    scene = await GetSceneImpl(sceneAddress, endpoint, cancellationToken);
                    scene.SceneConnectionStateObservable.Subscribe((state) =>
                    {
                        if(state.State == ConnectionState.Disconnecting)
                        {
                            _scenes.TryRemove(uSceneId, out _);
                        }
                    });
                }
                catch (System.Exception ex)
                {
                    _scenes.TryRemove(uSceneId, out _);
                    _logger.Log(Diagnostics.LogLevel.Error, "Client", $"Failed to get scene {sceneAddress.toUri()}", ex);
                    throw ex;
                }

                container.Task = Task.FromResult(scene);
                _scenes.TryAdd(uSceneId, container);
                return scene;
            }
        }

        public async Task<Scene> GetConnectedScene(string sceneId, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.Log(LogLevel.Trace, "Client", $"Get connected scene {sceneId}");

            if(string.IsNullOrEmpty(sceneId))
            {
                _logger.Log(LogLevel.Error, "Client", "SceneId is empty");
                throw new InvalidOperationException("SceneId is empty");
            }
            if(_scenes.TryGetValue(sceneId, out var scene))
            {
                return await scene.Task;
            }
            return null;
        }

        private async Task<SceneAddress> ParseSceneUrl(string url, CancellationToken cancellationToken)
        {
            var federation = await GetFederation(cancellationToken);
            return SceneAddress.Parse(url, federation.Current.Id, _config.Account, _config.Application);
        }

        private Task<Federation> GetFederation(CancellationToken cancellationToken)
        {
            var api = DependencyResolver.Resolve<ApiClient>();
            return api.GetFederation(new List<string>(_config.ServerEndpoints), cancellationToken);
        }

        private async Task<U> SendSystemRequest<T, U>(IConnection connection, byte id, T parameter)
        {
            var packet = await _requestProcessor.SendSystemRequest(connection, id, s =>
            {
                _systemSerializer.Serialize(parameter, s);
            });
            return _systemSerializer.Deserialize<U>(packet.Stream);
        }

        private async Task SendSystemRequestVoid<T>(IConnection connection, byte id, T parameter)
        {
            var packet = await _requestProcessor.SendSystemRequest(connection, id, s =>
            {
                _systemSerializer.Serialize(parameter, s);
            });
        }

        private Task UpdateServerMetadata()
        {
            return _requestProcessor.SendSystemRequest(_serverConnection, (byte)SystemRequestIDTypes.ID_SET_METADATA, s =>
            {
                _systemSerializer.Serialize(_serverConnection.Metadata, s);
            });
        }

        internal void ShutdownClient()
        {
            DependencyResolver.Resolve<ITransport>().Stop();
        }

        /// <summary>
        /// Returns a private scene (requires a token obtained from strong authentication with the Stormancer API).
        /// </summary>
        /// <remarks>
        /// The effective connection happens when "Connect" is called on the scene. Note that when you call GetScene, 
        /// a connection token is requested from the Stormancer API.this token is only valid for a few minutes: Don't get scenes
        /// a long time before connecting to them.
        /// </remarks>
        /// <param name="token">The token securing the connection.</param>
        /// <returns>A task returning the scene object on completion.</returns>        
        private async Task<Scene> GetSceneImpl(SceneAddress sceneAddress, SceneEndpoint sceneEndpoint, CancellationToken cancellationToken)
        {
            cancellationToken = GetLinkedCancellationToken(cancellationToken);

            try
            {
                _serverConnection = await EnsureConnectedToServer(sceneAddress.ClusterId, sceneEndpoint, cancellationToken);
            }
            catch (System.Exception ex)
            {
                Logger.Log(Diagnostics.LogLevel.Error, "Client", "GetPrivateScene an error occurred", ex.Message);
            }

            var parameter = new Stormancer.Dto.SceneInfosRequestDto { Metadata = _serverConnection.Metadata, Token = sceneEndpoint.Token };
            var result = await SendSystemRequest<Stormancer.Dto.SceneInfosRequestDto, Stormancer.Dto.SceneInfosDto>(_serverConnection, (byte)SystemRequestIDTypes.ID_GET_SCENE_INFOS, parameter);
            
            if (_serverConnection.DependencyResolver.Resolve<ISerializer>() == null)
            {
                if (result.SelectedSerializer == null)
                {
                    this._logger.Error("Client", "No serializer selected");
                    throw new InvalidOperationException("No serializer selected.");
                }
                _serverConnection.DependencyResolver.RegisterDependency(_serializers[result.SelectedSerializer]);
            }
            _serverConnection.Metadata["serializer"] = result.SelectedSerializer;
            this._logger.Info("Client", "Serializer selected: " + result.SelectedSerializer);

            await UpdateServerMetadata();
            
            var scene = new Scene(this._serverConnection, this, sceneAddress, sceneEndpoint.Token, result, DependencyResolver, new PluginBuildContext[] { _pluginCtx });
            scene.Initialize();

            return scene;
        }

        private IDisposable _syncClockSubscription;
        private void StartSyncClock()
        {
            if (this._syncClockSubscription != null)
            {
                return;
            }
            this._syncClockSubscription = this._scheduler.SchedulePeriodic(this._pingInterval, () =>
            {
                _ = this.SyncClockImpl();
            });
        }

        public void SetServerTimeout(TimeSpan timeout, CancellationToken ct)
        {
            _serverTimeout = timeout;
            _connections.SetTimeout(timeout, ct);

        }

        private async Task SyncClockImpl()
        {
            long timeStart = this._watch.ElapsedMilliseconds;

            try
            {
                var response = await this._requestProcessor.SendSystemRequest(this._serverConnection, (byte)SystemRequestIDTypes.ID_PING, s =>
                {
                    s.Write(BitConverter.GetBytes(timeStart), 0, 8);
                }, PacketPriority.IMMEDIATE_PRIORITY);
                ulong timeRef = 0;
                var timeEnd = this._watch.ElapsedMilliseconds;

                for (var i = 0; i < 8; i++)
                {
                    timeRef += (((ulong)response.Stream.ReadByte()) << (8 * i));
                }

                this.LastPing = timeEnd - timeStart;
                this._offset = (long)timeRef - (this.LastPing / 2) - timeStart;
                
            }
            catch (Exception e)
            {
                _logger.Log(Stormancer.Diagnostics.LogLevel.Error, "ping", "failed to ping server. " + e.Message);
            };
        }


        /// <summary>
        /// Returns a private scene (requires a token obtained from strong authentication with the Stormancer API.
        /// </summary>
        /// <remarks>
        /// The effective connection happens when "Connect" is called on the scene. Note that when you call GetScene, 
        /// a connection token is requested from the Stormancer API.this token is only valid for a few minutes: Don't get scenes
        /// a long time before connecting to them.
        /// </remarks>
        /// <param name="token">The token securing the connection.</param>
        /// <returns>A task returning the scene object on completion.</returns>
        public async Task<Scene> GetPrivateScene(string sceneToken, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _initializationTask;
            _logger.Log(Diagnostics.LogLevel.Trace, "Client", "Get private scene");
            if(sceneToken.Length == 0)
            {
                _logger.Log(Diagnostics.LogLevel.Error, "Client", "Empty scene token");
                throw new ArgumentException("Empty scene token");
            }

            SceneEndpoint sceneEndpoint = new SceneEndpoint();
            var tokenHandler = DependencyResolver.Resolve<ITokenHandler>();
            if(sceneToken.Substring(0,1) == "{")
            {
                sceneEndpoint = tokenHandler.GetSceneEndpointInfos(sceneToken);
            }
            else
            {
                sceneEndpoint = tokenHandler.DecodeToken(sceneToken);
            }
            var sceneId = sceneEndpoint.TokenData.SceneId;

            var sceneAddress = await ParseSceneUrl(sceneId, cancellationToken);
            var uSceneId = sceneAddress.toUri();
            ClientScene sceneContainer;
            if(_scenes.TryGetValue(uSceneId, out sceneContainer))
            {
                return await sceneContainer.Task;
            }
            else
            {
                sceneContainer = new ClientScene();
                sceneContainer.IsPublic = true;
                Scene scene;
                try
                {
                    scene = await GetSceneImpl(sceneAddress, sceneEndpoint, cancellationToken);
                }
                catch (Exception ex)
                {
                    _scenes.TryRemove(uSceneId, out _);
                    _logger.Log(LogLevel.Error, "Client", "An error occurred during GetPrivateScene : "+ex.Message, ex);
                    throw ex;
                }
                sceneContainer.Task = Task.FromResult(scene);
                _scenes.TryAdd(uSceneId, sceneContainer);
                return scene;
            }
        }

        internal async Task ConnectToScene(Scene scene, string token, IEnumerable<Route> localRoutes, CancellationToken ct = default(CancellationToken))
        {

            if(scene.ConnectionState.State != ConnectionState.Disconnected)
            {
                throw new InvalidOperationException("The scene is not in disconnected state.");
            }

            scene.SetConnectionState(new ConnectionStateCtx(ConnectionState.Connecting));

            var parameter = new Stormancer.Dto.ConnectToSceneMsg
            {
                Token = token,
                Routes = localRoutes.Select(r => new Stormancer.Dto.RouteDto
                {
                    Handle = r.Handle,
                    Metadata = r.Metadata,
                    Name = r.Name
                }).ToList(),
                ConnectionMetadata = _serverConnection.Metadata
            };
            try
            {
                var result = await this.SendSystemRequest<Stormancer.Dto.ConnectToSceneMsg, Stormancer.Dto.ConnectionResult>(_serverConnection, (byte)SystemRequestIDTypes.ID_CONNECT_TO_SCENE, parameter);
                this._logger.Log(Stormancer.Diagnostics.LogLevel.Trace, scene.Id, string.Format("Received connection result. Scene handle: {0}", result.SceneHandle));
                scene.CompleteConnectionInitialization(result);
                _scenesDispatcher.AddScene(_serverConnection, scene);
                scene.SetConnectionState(new ConnectionStateCtx(ConnectionState.Connected));
            }
            catch (Exception ex)
            {
                this._logger.Log(Stormancer.Diagnostics.LogLevel.Error, scene.Id, string.Format("Failed to connect to scene '{0}' : {1}", scene.Id, ex));
                throw;
            }
        }

        public async Task<Scene> ConnectToPublicScene(string sceneId, Action<Scene> sceneInitializer = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Scene scene = await GetPublicScene(sceneId, cancellationToken);
            if (scene.GetCurrentConnectionState() == ConnectionState.Disconnected)
            {
                sceneInitializer?.Invoke(scene);
            }
            var uSceneId = scene.Address.toUri();
            scene.SceneConnectionStateObservable.Subscribe((state) =>
            {
                if(state.State == ConnectionState.Disconnecting)
                {
                    _scenes.TryRemove(uSceneId, out _);
                }
            });

            try
            {
                await scene.Connect();
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, "Client", "Connection to public scene failed ", exception.Message);
                _scenes.TryRemove(uSceneId, out _);
                throw;
            }
            return scene;
        }

        public async Task<Scene> ConnectToPrivateScene(string sceneToken, Action<Scene> initializer, CancellationToken cancellationToken = default(CancellationToken))
        {
            SceneEndpoint endpoint = new SceneEndpoint();
            var tokenHandle = DependencyResolver.Resolve<ITokenHandler>();
            if(sceneToken.Substring(0,1) == "{")
            {
                endpoint = tokenHandle.GetSceneEndpointInfos(sceneToken);
            }
            else
            {
                endpoint = tokenHandle.DecodeToken(sceneToken);
            }
            var sceneId = endpoint.TokenData.SceneId;

            var scene = await GetPrivateScene(sceneToken, cancellationToken);
            if(scene.ConnectionState.State == ConnectionState.Disconnected)
            {
                initializer?.Invoke(scene);
            }
            var uSceneId = scene.Address.toUri();
            scene.SceneConnectionStateObservable.Subscribe((state) =>
            {
                if (state.State == ConnectionState.Disconnecting)
                {
                    _scenes.TryRemove(uSceneId, out _);
                }
            });
            try
            {
                await scene.Connect();
            }
            catch (System.Exception ex)
            {
                _logger.Log(LogLevel.Error, "Client", "An Error occurred during connection to private scene", ex.Message);
                _scenes.TryRemove(uSceneId, out _);
                throw ex;
            }
            return scene;
        }

        public async Task Disconnect(IConnection connection, byte sceneHandle, bool initiatedByServer, string reason)
        {
            var sceneDispatcher = DependencyResolver.Resolve<SceneDispatcher>();
            var scene = sceneDispatcher.GetScene(connection, sceneHandle);

            await Disconnect(scene, initiatedByServer, reason);
        }


        public async Task Disconnect(string sceneId, bool initiatedByServer, string reason = "")
        {
            ClientScene clientScene;
            if(_scenes.TryGetValue(sceneId, out clientScene))
            {
                var scene = await clientScene.Task;
                await Disconnect(scene, initiatedByServer, reason);
            }
        }

        public async Task Disconnect(Scene scene, bool initiatedByServer = false, string reason = "")
        {
            if (scene == null)
            {
                throw new InvalidOperationException("Scene deleted");
            }

            if(scene.GetCurrentConnectionState() != ConnectionState.Connected)
            {
                throw new InvalidOperationException("The scene is not in connected state");
            }

            var sceneId = scene.Address.toUri();
            var sceneHandle = scene.Host.Handle;

            _scenes.TryRemove(sceneId, out  _);
            scene.SetConnectionState(new ConnectionStateCtx(ConnectionState.Disconnecting, reason));

            var sceneDispatcher = DependencyResolver.Resolve<SceneDispatcher>();
            
            if(sceneDispatcher == null || _connections == null)
            {
                throw new InvalidOperationException("Client destroyed");
            }
            IConnection connection = _connections.GetConnection(scene.Host.Id);
            if(connection != null)
            {
                sceneDispatcher.RemoveScene(_serverConnection, sceneHandle);
                if (!initiatedByServer)
                {
                    await SendSystemRequestVoid(connection, (byte)SystemRequestIDTypes.ID_DISCONNECT_FROM_SCENE, sceneHandle);
                }
                Logger.Log(LogLevel.Debug, "Client", "Scene disconnected", sceneId);
                scene.SetConnectionState(new ConnectionStateCtx(ConnectionState.Disconnected, reason));
            }
        }


        /// <summary>
        /// Disconnects the client.
        /// </summary>
        public void Disconnect()
        {
            _logger.Log(LogLevel.Trace, "Client", "Disconnecting client");
            using (this._syncClockSubscription)
            { };
            if (_serverConnection != null)
            {
                _serverConnection.Close();
            }

        }

        private bool _disposed;
        private ITransport _transport;

        /// <summary>
        /// Disposes the client object.
        /// </summary>
        /// <remarks>
        /// Calls the *Disconnect* method  to shutdown the transport gracefully.
        /// </remarks>
        public void Dispose()
        {
            if (!this._disposed)
            {
                this._disposed = true;
                Disconnect();

                var ev = _pluginCtx.ClientDestroyed;
                if (ev != null)
                {
                    ev(this);
                }
            }

        }

        private async Task<IConnection> EnsureConnectedToServer(string clusterId, SceneEndpoint sceneEndpoint, CancellationToken ct)
        {
            return await _connections.GetConnection(clusterId, async (id) =>
            {
                try
                {
                    var transport = DependencyResolver.Resolve<ITransport>();
                    List<string> endpoints;
                    if(!sceneEndpoint.TokenResponse.Endpoints.TryGetValue(transport.Name, out endpoints))
                    {
                        throw new IndexOutOfRangeException($"No transfport of type {transport.Name} available on this server");
                    }
                    var random = new Random();
                    var endpoint = endpoints[random.Next(endpoints.Count)];
                    _logger.Log(Diagnostics.LogLevel.Trace, "Client", "Connecting transport to server", endpoint);
                    var connection = await transport.Connect(endpoint, id, "", ct);
                    connection.Metadata = _metadata;
                    connection.Metadata["type"] =  "server";

                   if (_config.EncryptionEnabled)
                    {
                        connection.Metadata["encryption"] =  sceneEndpoint.TokenResponse.Encryption.Token;
                        var keyStore = DependencyResolver.Resolve<KeyStore>();
                        var key = sceneEndpoint.TokenResponse.Encryption.Key;
                        if (key.Length != 256 / 8)
                        {
                            throw new InvalidOperationException("Unexpected key size. received " + key.Length * 8 + " bits expected 256 bits ");
                        }
                        keyStore.Keys.Add(connection.Id, Encoding.ASCII.GetBytes(key));
                    }

                    await connection.SetTimeout(_serverTimeout, ct);
                    await connection.UpdatePeerMetadata(ct);
                    await RequestSessionToken(connection, 1, ct);
                    return connection;
                }
                catch (System.Exception ex)
                {
                    throw new InvalidOperationException($"Failed to establish connection with {id} : {ex.Message}");
                }
            });
        }

        private CancellationToken GetLinkedCancellationToken(CancellationToken cancellationToken)
        {
            return CancellationTokenHelpers.CreateLinkedSource(cancellationToken, _cts.Token, Shutdown.Instance.GetShutdownToken()).Token;
        }

        private async Task RequestSessionToken(IConnection peer, int numRetries, CancellationToken ct)
        {
            if(peer == null)
            {
                throw new InvalidOperationException("Peer disconnected");
            }
            var requestProcessor = DependencyResolver.Resolve<RequestProcessor>();
            Task<string> task;
            if(_sessionToken == null)
            {
                task = requestProcessor.SendSystemRequest<string>(peer, (byte)SystemRequestIDTypes.ID_JOIN_SESSION, ct);
            }
            else
            {
                task = requestProcessor.SendSystemRequest<string, string>(peer, (byte)SystemRequestIDTypes.ID_JOIN_SESSION, _sessionToken, ct);
            }
            try
            {
                var serverToken = await task;
                if(_sessionToken == null)
                {
                    _sessionToken = serverToken;
                }
            }
            catch (Exception ex)
            {
                _sessionToken = "";
                if(numRetries > 0)
                {
                    _logger.Log(Diagnostics.LogLevel.Warn, "Client.RequestSessionToken", $"Server returned an error: {ex.Message}, trying {numRetries} more time(s)");
                    await RequestSessionToken(peer, numRetries - 1, ct);
                    return;
                }
                throw ex;
            }
        }

        /// <summary>
        /// The server connection's ping, in milliseconds.
        /// </summary>
        public int ServerPing { get { return this._serverConnection.Ping; } }

        /// <summary>
        /// The name of the transport used for connecting to the server.
        /// </summary>
        public string ServerTransportType { get { return this._transport.Name; } }

        /// <summary>
        /// Returns statistics about the connection to the server.
        /// </summary>
        /// <returns>The required statistics</returns>
        public IConnectionStatistics GetServerConnectionStatistics()
        {
            return this._serverConnection.GetConnectionStatistics();
        }
    }
}
