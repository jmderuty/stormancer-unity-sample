using RakNet;
using Stormancer.Core;
using Stormancer.Diagnostics;
using Stormancer.Plugins;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stormancer.Networking
{
    public class RakNetTransport : ITransport
    {
        private class PendingPing
        {
            public string Address { get; set; }
            public TaskCompletionSource<bool> Tcs { get; set; }

            public PendingPing(string address, TaskCompletionSource<bool> tcs)
            {
                Address = address;
                Tcs = tcs;
            }
        }

        private IConnectionManager _handler;
        private RakPeerInterface _peer;
        private ILogger _logger;
        private string _type;
        private ushort _port = 0;
        private readonly ConcurrentDictionary<ulong, RakNetConnection> _connections = new ConcurrentDictionary<ulong, RakNetConnection>();
        private readonly ConcurrentDictionary<string, TaskCompletionSource<int>> _pendingPings = new ConcurrentDictionary<string, TaskCompletionSource<int>>();
        private readonly ConcurrentQueue<PendingPing> _pendingPingsQueue = new ConcurrentQueue<PendingPing>();
        private readonly IConnectionHandler _connectionHandler;
        private IDependencyResolver _dependencyResolver;

        private readonly object _connectionTaskSyncRoot = new object();
        private Task<IConnection> _connectionTask = TaskHelper.FromResult(default(IConnection));
        private uint _socketDescriptorCount = 0;

        public RakNetTransport(IDependencyResolver resolver)
        {
            _dependencyResolver = resolver;
            _logger = resolver.Resolve<ILogger>();
            _connectionHandler = resolver.Resolve<IConnectionHandler>();
        }
        public Task Start(string type, IConnectionManager handler, CancellationToken token, ushort? serverPort, ushort maxConnections, AddressType addressType = AddressType.Undefined)
        {
            _logger.Log(LogLevel.Trace, "RakNetTransport", "Starting RakNet transport...");

            if (handler == null && serverPort.HasValue)
            {
                throw new ArgumentNullException("Handler is null for server.");
            }
            _type = type;
            _handler = handler;
            //Initialize(maxConnections, serverPort);
            

            var tcs = new TaskCompletionSource<bool>();
            Task.Factory.StartNew(() => Run(token, serverPort, maxConnections, tcs, addressType));
            return tcs.Task;
        }

        private const int connectionTimeout = 5000;

        private void Run(CancellationToken token, ushort? serverPort, ushort maxConnections, TaskCompletionSource<bool> startupTcs, AddressType addressType)
        {
            IsRunning = true;
            _logger.Info("Starting raknet transport " + _type);
            RakPeerInterface server;
            try
            {
                server = RakPeerInterface.GetInstance();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "RakNetTransport", "An error occurred during Run : "+ex.Message, ex);
                throw;
            }

            var socketDescriptorList = new RakNetListSocketDescriptor();

            bool forceIpv4 = false;
#if (!UNITY_IOS || UNITY_EDITOR || UNITY_STANDALONE)
            if (addressType != AddressType.Ipv4)
            {
                forceIpv4 = true;
            }
#endif

            if ((Socket.OSSupportsIPv4 && addressType == AddressType.Ipv4) || forceIpv4)
            {
                _logger.Debug("using address type Ipv4");
                var socketDescriptorIpv4 = new SocketDescriptor(serverPort.GetValueOrDefault(), null);
                socketDescriptorList.Push(socketDescriptorIpv4, null, 0);
            }

#if (!UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE)
            if (Socket.OSSupportsIPv6 && addressType == AddressType.Ipv6)
            {
                _logger.Debug("using address type Ipv6");
                var socketDescriptorIpv6 = new SocketDescriptor(serverPort.GetValueOrDefault(), null);
#if (UNITY_IOS && !UNITY_EDITOR && !UNITY_STANDALONE)
			    socketDescriptorIpv6.socketFamily = 30; // AF_INET6
#else
                socketDescriptorIpv6.socketFamily = 23; // AF_INET6
#endif
                socketDescriptorList.Push(socketDescriptorIpv6, null, 0);
            }
#endif
            _logger.Debug($"socket descriptor list size : {socketDescriptorList.Size()}");
            var startupResult = server.Startup(maxConnections, socketDescriptorList, 1);
            if(startupResult == StartupResult.RAKNET_ALREADY_STARTED)
            {
                _logger.Error("raknet server already started. Shuting down actual server and create new one");
                server.Shutdown(1000);
                RakPeerInterface.DestroyInstance(server);
                IsRunning = false;
                _logger.Info("Stopped raknet server.");

                // start new one
                IsRunning = true;
                _logger.Info("Starting raknet transport " + _type);
                try
                {
                    server = RakPeerInterface.GetInstance();
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, "RakNetTransport", "An error occurred during Run : "+ex.Message, ex);
                    throw;
                }
                startupResult = server.Startup(maxConnections, socketDescriptorList, 1);
            }

            if (startupResult != StartupResult.RAKNET_STARTED)
            {
                _logger.Error("Couldn't start raknet peer :" + startupResult);
                throw new InvalidOperationException("Couldn't start raknet peer :" + startupResult);
            }

            server.SetMaximumIncomingConnections(maxConnections);
            _socketDescriptorCount = socketDescriptorList.Size();
            _peer = server;
            startupTcs.SetResult(true);
            _logger.Info("Raknet transport " + _type + " started");
            while (!token.IsCancellationRequested)
            {
                for (var packet = server.Receive(); packet != null; packet = server.Receive())
                {



                    switch (packet.data[0])
                    {
                        case (byte)DefaultMessageIDTypes.ID_CONNECTION_REQUEST_ACCEPTED:
                            if(_pendingConnections.Count == 0)
                            {
                                _logger.Log(LogLevel.Error, "RakNetTransport", "Can't get the pending connection TCS", packet.systemAddress.ToString(true, ':'));
                            }
                            else
                            {

                                IConnection c;
                                PendingConnection pendingConnection;
                                _pendingConnections.TryPeek(out pendingConnection);

                                if(pendingConnection.CancellationToken.IsCancellationRequested)
                                {
                                    _peer.CloseConnection(packet.guid, false);
                                }
                                else if(pendingConnection.IsP2P)
                                {
                                    var parentConnection = _handler.GetConnection(pendingConnection.ParentId);

                                    BitStream stream = new BitStream();
                                    stream.Write((byte)MessageIDTypes.ID_ADVERTISE_PEERID);
                                    stream.Write(parentConnection.Id);
                                    stream.Write(pendingConnection.ParentId);
                                    stream.Write(pendingConnection.Id);
                                    stream.Write(true);
                                    _peer.Send(stream, RakNet.PacketPriority.MEDIUM_PRIORITY, RakNet.PacketReliability.RELIABLE, (char)0, packet.guid, false);
                                }
                            }
                            break;
                        case (byte)DefaultMessageIDTypes.ID_NO_FREE_INCOMING_CONNECTIONS:
                            ConnectionAttemptFailed(packet, "Connection failed because too many concurrent connections.");
                            break;
                        case (byte)DefaultMessageIDTypes.ID_INCOMPATIBLE_PROTOCOL_VERSION:
                        case (byte)DefaultMessageIDTypes.ID_CONNECTION_ATTEMPT_FAILED:
                            ConnectionAttemptFailed(packet);
                            break;

                        case (byte)DefaultMessageIDTypes.ID_ALREADY_CONNECTED:
                            _logger.Log(LogLevel.Error, "RakNetTransport", "peer already connected", packet.systemAddress.ToString(true, ':'));
                            break;

                        case (byte)DefaultMessageIDTypes.ID_DISCONNECTION_NOTIFICATION:
                            _logger.Trace("{0} disconnected.", packet.systemAddress.ToString());
                            OnDisconnection(packet, server, "CLIENT_DISCONNECTED");
                            break;
                        case (byte)DefaultMessageIDTypes.ID_CONNECTION_LOST:
                            _logger.Trace("{0} lost the connection.", packet.systemAddress.ToString());
                            OnDisconnection(packet, server, "CONNECTION_LOST");
                            break;
                        case (byte)DefaultMessageIDTypes.ID_CONNECTION_BANNED:
                            _logger.Log(LogLevel.Trace, "RakNetTransport", "We are banned from the system we attempted to connect to", packet.systemAddress.ToString(true, ':'));
                            break;
                        case (byte)DefaultMessageIDTypes.ID_INVALID_PASSWORD:
                            _logger.Log(LogLevel.Trace, "RakNetTransport", "The remote system is using a password and has refused our connection because we did not set the correct password", packet.systemAddress.ToString(true, ':'));
                            break;
                        case (byte)DefaultMessageIDTypes.ID_IP_RECENTLY_CONNECTED:
                            _logger.Log(LogLevel.Trace, "RakNetTransport", "this IP address connected recently, and can't connect again as a security measure", packet.systemAddress.ToString(true, ':'));
                            break;
                        case (byte)DefaultMessageIDTypes.ID_UNCONNECTED_PONG:
                            throw new NotImplementedException();
					    case (byte)DefaultMessageIDTypes.ID_UNCONNECTED_PING_OPEN_CONNECTIONS:
                            break;
					    case (byte)DefaultMessageIDTypes.ID_ADVERTISE_SYSTEM:
                            _logger.Log(LogLevel.Trace, "RakNetTransport", "Inform a remote system of our IP/Port", packet.systemAddress.ToString(true, ':'));
                            break;
					    // Stormancer messages types
					    case (byte)MessageIDTypes.ID_CONNECTION_RESULT:
                            {
                                string packetSystemAddressStr = packet.systemAddress.ToString(true, ':');
                                if (_pendingConnections.Count > 0)
                                {
                                    PendingConnection pending;
                                    if(_pendingConnections.TryDequeue(out pending))
                                    {
                                        ulong sid = BitConverter.ToUInt64(packet.data, 1);


                                        _logger.Log(LogLevel.Trace, "RakNetTransport", "Successfully connected to ", packet.systemAddress.ToString());

                                        var connection = OnConnection(packet.systemAddress, packet.guid, sid, pending);
                                        StartNextPendingConnections();
                                    }

                                }
                            }
                            break;
                        case (byte)MessageIDTypes.ID_CLOSE_REASON:
                            {
                                var connection = GetConnection(packet.guid);
                                if(connection != null)
                                {
                                    using (BitStream stream = new BitStream(packet.data, packet.length, true))
                                    {
                                        string reason;
                                        stream.Read(out reason);
                                        connection.CloseReason = reason;
                                    }
                                }
                            }
                            break;
                        case (byte)DefaultMessageIDTypes.ID_UNCONNECTED_PING:
                        	break;
                        case (byte)MessageIDTypes.ID_ADVERTISE_PEERID:
                            {
                                string packetSystemAddressStr = packet.systemAddress.ToString(true, ':');
                                string parentId;
                                byte[] buffer;
                                string id;
                                bool requestResponse = false;
                                ulong remotePeerId = 0;
                                
                                using (MemoryStream stream = new MemoryStream(packet.data))
                                {
                                    using (BinaryReader reader = new BinaryReader(stream))
                                    {
                                        // read the first byte for the MessageIdType
                                        reader.ReadByte();

                                        remotePeerId = reader.ReadUInt64();
                                        parentId = reader.ReadString();
                                        id = reader.ReadString();
                                        requestResponse = reader.ReadBoolean();
                                    }

                                    if(!requestResponse)
                                    {
                                        PendingConnection pending;
                                        if(_pendingConnections.TryDequeue(out pending))
                                        {
                                            if(pending.CancellationToken.IsCancellationRequested)
                                            {
                                                _peer.CloseConnection(packet.guid, false);
                                            }
                                            else
                                            {
                                                _logger.Log(LogLevel.Trace, "RakNetTransport", "Connection request accepted", packetSystemAddressStr);
                                                var connection = OnConnection(packet.systemAddress, packet.guid, remotePeerId, pending);
                                            }
                                        }
                                        StartNextPendingConnections();
                                    }
                                    else
                                    {
                                        var parentConnection = _handler.GetConnection(parentId);
                                        using (BitStream data = new BitStream())
                                        {
                                            data.Write((byte)MessageIDTypes.ID_ADVERTISE_PEERID);
                                            data.Write(parentConnection.Id);
                                            data.Write(parentId);
                                            data.Write(id);
                                            data.Write(false);
                                            _peer.Send(data, RakNet.PacketPriority.MEDIUM_PRIORITY, RakNet.PacketReliability.RELIABLE, '0', packet.guid, false);
                                        }
                                    }
                                    
                                }
                            }
                            break;
                        default:
                            OnMessageReceived(packet);
                            break;
                    }
                    if(packet != null && _peer.IsActive())
                    {
                        _peer.DeallocatePacket(packet);
                    }
                }
                
                Thread.Sleep(5);
            }
            server.Shutdown(1000);
            RakPeerInterface.DestroyInstance(server);
            IsRunning = false;
            _logger.Info("Stopped raknet server.");
        }

        private void ConnectionAttemptFailed(RakNet.Packet packet, string v = null)
        {
            if(v != null)
            {
                _logger.Log(LogLevel.Error, "RaknetTransport", v);
            }
            string packetAddress = packet.systemAddress.ToString(true, ':');
            if (_pendingConnections.Count == 0)
            {
                _logger.Log(LogLevel.Error, "RakNetTransport", "Can't get the pending connection TCS", packetAddress);
            }
            else
            {
                PendingConnection pendingConnection;
                _pendingConnections.TryDequeue(out pendingConnection);
                if (!pendingConnection.CancellationToken.IsCancellationRequested)
                {
                    _logger.Log(LogLevel.Trace, "RakNetTransport", "Connection request failed", packetAddress);
                    pendingConnection.Tcs.SetException(new InvalidOperationException("Connection attempt failed"));
                }
                StartNextPendingConnections();
            }
        }

        private void OnConnectionIdReceived(long p)
        {
            Id = p;
        }

#region message handling

        private IConnection OnConnection(RakNet.SystemAddress systemAddress, RakNetGUID guid, ulong peerId, PendingConnection request)
        {
            _logger.Trace("Connected to endpoint {0}", systemAddress);

            IConnection connection = CreateNewConnection(guid, peerId, request.Id);
            var metadata = connection.Metadata;
            var ctx = new PeerConnectedContext { Connection = connection };

            _connectionHandler.PeerConnected?.Invoke(ctx);

            connection = ctx.Connection;
            _handler.NewConnection(connection);

            ConnectionOpened?.Invoke(connection);

            connection.SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Connected, ""));
            request.Tcs.SetResult(connection);
            return connection;
        }



        private IConnection OnConnection(RakNet.SystemAddress systemAddress, RakNetGUID guid, ulong peerId, string connectionId)
        {
            _logger.Trace("Connected to endpoint {0}", systemAddress);

            IConnection connection = CreateNewConnection(guid, peerId, connectionId);
            var ctx = new PeerConnectedContext { Connection = connection };
            
            _connectionHandler.PeerConnected?.Invoke(ctx);
            

            connection = ctx.Connection;
            _handler.NewConnection(connection);
            
            ConnectionOpened?.Invoke(connection);

            connection.SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Connected, ""));
            return connection;
        }


        private void OnDisconnection(RakNet.Packet packet, RakPeerInterface server, string reason)
        {
            _logger.Trace("Disconnected from endpoint {0}", packet.systemAddress);
            var connection = RemoveConnection(packet.guid.g);

            _handler.CloseConnection(connection, reason);
            
            if (connection != null)
            {
                connection.OnClose?.Invoke(reason);
            }

            connection.SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Disconnected, reason));
        }

        private void OnMessageReceived(RakNet.Packet packet)
        {
            //var messageId = packet.data[0];
            var connection = GetConnection(packet.guid);
            var stream = new MemoryStream((int)packet.length);
            //var buffer = new byte[packet.data.Length];
            stream.Write(packet.data, 0, (int)packet.length);
            stream.Seek(0, SeekOrigin.Begin);
            //logger.Trace("message arrived: [{0}]", string.Join(", ", buffer.Select(b => b.ToString()).ToArray()));

            var p = new Stormancer.Core.Packet(
                               connection,
                               stream);


            this.PacketReceived(p);
        }
#endregion

#region manage connections
        private RakNetConnection GetConnection(RakNetGUID guid)
        {
            return _connections[guid.g];
        }

        private RakNetConnection CreateNewConnection(RakNetGUID raknetGuid, ulong peerId, string key)
        {
            if(_peer != null)
            {
                var cid = peerId;
                var connection = new RakNetConnection(raknetGuid, peerId, key, _peer, _logger, _dependencyResolver);
                connection.OnClose += (reason) =>
                {
                    _peer.CloseConnection(raknetGuid, true);
                };
                _connections.TryAdd(raknetGuid.g, connection);
                return connection;
            }
            return null;
        }

        private RakNetConnection RemoveConnection(ulong guid)
        {
            RakNetConnection connection;
            _connections.TryRemove(guid, out connection);
            return connection;
        }

        private void OnRequestClose(RakNetConnection c)
        {
            if (_peer != null)
            {
                _peer.CloseConnection(c.Guid, true);
            }
        }

#endregion


        public Action<Stormancer.Core.Packet> PacketReceived
        {
            get;
            set;
        }

        public Action<IConnection> ConnectionOpened
        {
            get;
            set;
        }

        public async Task<IConnection> Connect(string endpoint, string id, string parentId)
        {
            return await Connect(endpoint, id, parentId, CancellationToken.None);
        }

        public async Task<IConnection> Connect(string endpoint, string id, string parentId, CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<IConnection>();
            var shouldStart = _pendingConnections.Count == 0;
            var pendingConnection = new PendingConnection();
            pendingConnection.Endpoint = endpoint;
            pendingConnection.CancellationToken = ct;
            pendingConnection.Id = id;
            pendingConnection.ParentId = parentId;
            _pendingConnections.Enqueue(pendingConnection);
            if(shouldStart)
            {
                StartNextPendingConnections();
            }

            var connection = await pendingConnection.Tcs.Task;
            connection.SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Connecting, ""));
            return connection;
        }

        private void StartNextPendingConnections()
        {
            if(_pendingConnections.Count == 0)
            {
                return;
            }

            PendingConnection pending;
            _pendingConnections.TryPeek(out pending);

            if(pending.CancellationToken.IsCancellationRequested)
            {
                _pendingConnections.TryDequeue(out pending);
                StartNextPendingConnections();
            }

            var endpoint = pending.Endpoint;
            var split = endpoint.Split(':');
            var tcs = pending.Tcs;
            if(split.Length < 2)
            {
                tcs.SetException(new ArgumentException($"Bad server endpoint, no port ({endpoint})"));
                return;
            }
            string portString = split[1];
            if (endpoint.Length - 1 <= portString.Length)
            {
                tcs.SetException(new ArgumentException(($"Bad server endpoint, no host ({endpoint})")));
                return;
            }
            
            _port = UInt16.Parse(portString);
            if (_port == 0)
            {
                tcs.SetException(new InvalidOperationException(($"Server endpoint port should not be 0 ({endpoint})")));
                return;
            }

            var hostStr = split[0];

            if (_peer == null || !_peer.IsActive())
            {
                tcs.SetException(new InvalidOperationException("Transport not started. Make sure you started it."));
                return;
            }

            var result = _peer.Connect(hostStr, _port, null, 0, null, 0, 12, 500, 30000);
            if (result != ConnectionAttemptResult.CONNECTION_ATTEMPT_STARTED)
            {
                tcs.SetException(new InvalidOperationException($"Bad RakNet connection attempt result ({result})"));
                return;
            }
        }

        private ConcurrentQueue<PendingConnection> _pendingConnections = new ConcurrentQueue<PendingConnection>();

        public string Name
        {
            get { return "raknet"; }
        }


        public bool IsRunning
        {
            get;
            private set;
        }

        public long? Id { get; private set; }

        public void Stop()
        {
            _logger.Log(LogLevel.Trace, "RaknetTransport", "Stopping RakNet transport ...");

            List<ulong> guids = new List<ulong>();
            foreach (var connection in _connections)
            {
                guids.Add(connection.Key);
            }
            foreach (var pId in guids)
            {
                OnDisconnection(pId, "CLIENT_TRANSPORT_STOPPED");
            }
        }

        private void OnDisconnection(ulong guid, string reason)
        {
            var connection = RemoveConnection(guid);

            if(connection != null)
            {
                if(connection.CloseReason != "")
                {
                    reason = connection.CloseReason;
                }

                _handler.CloseConnection(connection, reason);
                connection.OnClose(reason);
                connection.SetConnectionState(new ConnectionStateCtx(Core.ConnectionState.Disconnected, reason));
            }
            else
            {
                _logger.Log(LogLevel.Warn, "RakNetTransport", $"Attempting to disconnect from unknown peer (guid: {guid})");
            }
        }

        public List<string> GetAvailableEndpoints()
        {
            var nbIps = _peer.GetNumberOfAddresses();
            List<string> endpoints = new List<string>();
            for(uint i = 0; i <nbIps; i++)
            {
                var boundAddress = _peer.GetMyBoundAddress(0);
                var ip = _peer.GetLocalIP(i);
                var address = IPAddress.Parse(ip);
                if(address.AddressFamily == AddressFamily.InterNetwork)
                {
                    endpoints.Add($"{ip}:{boundAddress.GetPort()}");
                }
            }
            return endpoints;
        }

        public async Task<int> SendPing(string address, int number, CancellationToken cancellationToken)
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            if(!_pendingPings.TryAdd(address, tcs))
            {
                throw new InvalidOperationException($"Could not insert {address} into the pending pings");
            }
            var eventSetTask = tcs.Task;
            CancellationTokenSource cts = cancellationToken.CanBeCanceled ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken) : new CancellationTokenSource();
            List<Task<bool>> tasks = new List<Task<bool>>();
            for(int i = 0; i < number; i++)
            {

                await Task.Delay(300 * i, cts.Token);
                var task = SendPingImplTask(address);
                tasks.Add(task);
                await task;
            }
            var results = await Task.WhenAll(tasks);
            cts.Token.ThrowIfCancellationRequested();
            bool sent = false;
            foreach(bool result in results)
            {
                if(result)
                {
                    sent = true;
                    break;
                }
            }
            if(!sent)
            {
                _logger.Log(LogLevel.Error, "RakNetTransport", $"Pings to {address} failed : unreachable address");
                tcs.SetResult(-1);
            }

            try
            {
                var result = await tcs.Task.TimeOut(1500);
                TaskCompletionSource<int> taskSource;
                if(_pendingPings.TryRemove(address, out taskSource))
                {
                    taskSource.SetCanceled();
                }
                return result;
            }
            catch (System.Exception ex)
            {
                _logger.Log(LogLevel.Error, "RakNetTransport", $"Ping to {address} failed : {ex.Message}");
                return -1;
            }
        }

        private Task<bool> SendPingImplTask(string address)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            _pendingPingsQueue.Enqueue(new PendingPing(address, tcs));
            return tcs.Task;
        }

        public async Task<int> SendPing(string address, CancellationToken cancellationToken)
        {
            return await SendPing(address, 2, cancellationToken);
        }

        public async Task<int> SendPing(string address)
        {
            return await SendPing(address, CancellationToken.None);
        }

        public async Task<int> SendPing(string address, int number)
        {
            return await SendPing(address, number, CancellationToken.None);
        }

        public void OpenNat(string address)
        {
            var els = address.Split(':');
            _peer.SendTTL(els[0], Convert.ToUInt16(els[1]), 3);
        }
    }



}
