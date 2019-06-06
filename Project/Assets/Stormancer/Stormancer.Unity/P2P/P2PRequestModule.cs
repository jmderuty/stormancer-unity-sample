using Stormancer.Client45;
using Stormancer.Networking;
using System.Collections.Generic;
using System;
using System.Text;
using Stormancer.Diagnostics;
using Stormancer.Core;
using Stormancer.Plugins;
using System.Threading.Tasks;

namespace Stormancer
{
    public class P2PRequestModule : IRequestModule
    {
        private ITransport _transport;
        private IConnectionManager _connections;
        private P2PSessions _sessions;
        private ISerializer _serializer;
        private P2PTunnels _tunnels;
        private ILogger _logger;
        private ClientConfiguration _config;
        private Client _client;

        public P2PRequestModule(ITransport transport, IConnectionManager connections, P2PSessions sessions, ISerializer serializer, P2PTunnels tunnels, ILogger logger, ClientConfiguration config, Client client)
        {
            _transport = transport;
            _connections = connections;
            _sessions = sessions;
            _serializer = serializer;
            _tunnels = tunnels;
            _logger = logger;
            _config = config;
            _client = client;
        }

        public void RegisterModule(RequestModuleBuilder builder)
        {
            builder.Service((byte)SystemRequestIDTypes.ID_P2P_GATHER_IP, context =>
            {
                List<string> endpoints = new List<string>();
                if (!string.IsNullOrEmpty(_config.DedicatedServerEndpoint))
                {
                    endpoints.Add(_config.DedicatedServerEndpoint + ":" + _config.ClientSDKPort);
                }
                else if (!_config.EnableNatPunchthrough)
                {
                    endpoints.Clear();
                }
                else
                {
                    endpoints = _transport.GetAvailableEndpoints();
                }
                context.Send(stream =>
                {
                    _serializer.Serialize(endpoints, stream);
                });
                return Task.CompletedTask;
            });

            builder.Service((byte)SystemRequestIDTypes.ID_DISCONNECT_FROM_SCENE, async context =>
            {
                var sceneId = _serializer.Deserialize<string>(context.InputStream);
                var reason = _serializer.Deserialize<string>(context.InputStream);
                var sceneHandle = _serializer.Deserialize<byte>(context.InputStream);

                await _client.Disconnect(context.Packet.Connection, sceneHandle, true, reason);

            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_CREATE_SESSION, context =>
            {
                var sessionId = _serializer.Deserialize<byte[]>(context.InputStream);
                var peerId = _serializer.Deserialize<ulong>(context.InputStream);
                var sceneId = _serializer.Deserialize<string>(context.InputStream);
                P2PSession session = new P2PSession();
                session.SessionId = sessionId;
                session.SceneId = sceneId;
                session.RemotePeer = peerId;
                _sessions.CreateSession(P2PSessionId.From(sessionId), session);
                context.Send(stream => { });
                return Task.CompletedTask;
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_CLOSE_SESSION, context =>
            {
                var sessionId = _serializer.Deserialize<byte[]>(context.InputStream);
                _sessions.CloseSession(P2PSessionId.From(sessionId));
                context.Send(stream => { });
                return Task.CompletedTask;
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_TEST_CONNECTIVITY_CLIENT, async context =>
            {
                var candidate = _serializer.Deserialize<ConnectivityCandidate>(context.InputStream);

                _logger.Log(LogLevel.Debug, "p2p", "Starting connectivity test (CLIENT) ");

                var connection = _connections.GetConnection(candidate.ListeningPeer);
                if (connection != null && connection.GetConnectionState().State == ConnectionState.Connected)
                {
                    context.Send(stream =>
                    {
                        _serializer.Serialize(0, stream);
                    });
                    return;
                }
                if (!_config.EnableNatPunchthrough)
                {
                    if (candidate.ListeningEndpointCandidate.Address.Substring(0, 9) == "127.0.0.1")
                    {
                        context.Send(stream =>
                        {
                            _serializer.Serialize(-1, stream);
                        });
                        return;
                    }
                    else
                    {
                        context.Send(stream =>
                        {
                            _serializer.Serialize(1, stream);
                        });
                        return;
                    }
                }
                else if (!string.IsNullOrEmpty(_config.DedicatedServerEndpoint))
                {
                    context.Send(stream =>
                    {
                        _serializer.Serialize(-1, stream);
                    });
                    return;
                }
                else
                {
                    var latency = await _transport.SendPing(candidate.ListeningEndpointCandidate.Address);
                    _logger.Log(LogLevel.Debug, "p2p", "Connectivity test complete ping : " + latency);
                    context.Send(stream =>
                    {
                        _serializer.Serialize(latency, stream);
                    });
                }
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_TEST_CONNECTIVITY_HOST, context =>
            {
                var candidate = _serializer.Deserialize<ConnectivityCandidate>(context.InputStream);
                _logger.Log(LogLevel.Debug, "p2p", "Starting connectivity test (Listener)");

                var connection = _connections.GetConnection(candidate.ClientPeer);
                if (connection != null && connection.GetConnectionState().State == ConnectionState.Connected)
                {
                    context.Send(stream => { });
                    return Task.CompletedTask;
                }
                if (string.IsNullOrEmpty(_config.DedicatedServerEndpoint) && _config.EnableNatPunchthrough)
                {
                    _transport.OpenNat(candidate.ClientEndpointCandidate.Address);
                }
                context.Send(stream => { });
                return Task.CompletedTask;
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_CONNECT_HOST, context =>
            {
                var candidate = _serializer.Deserialize<ConnectivityCandidate>(context.InputStream);
                var connection = _connections.GetConnection(candidate.ClientPeer);
                _logger.Log(LogLevel.Debug, "P2P", $"ClientPeer connection {connection?.ToString()}");
                var sessionId = P2PSessionId.From(candidate.SessionId);
                if (connection != null && connection.GetConnectionState().State == ConnectionState.Connected)
                {
                    _logger.Log(LogLevel.Debug, "p2p", "Connection already existing and connected");
                    _sessions.UpdateSessionState(sessionId, P2PSessionState.Connected);
                }
                else
                {
                    _logger.Log(LogLevel.Debug, "p2p", "Waiting connection");


                    _ = _connections.AddPendingConnection(candidate.ClientPeer).ContinueWith(t =>
                    {
                        try
                        {
                            var co = t.Result;
                            co.Metadata["type"] = "p2p";
                            _sessions.UpdateSessionState(sessionId, P2PSessionState.Connected);
                        }
                        catch (Exception ex)
                        {
                            _logger.Log(LogLevel.Error, "p2p.connect_host", $"An error occured {ex.Message}");
                        }
                    });
                }

                context.Send(stream => { });
                return Task.CompletedTask;
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_CONNECT_CLIENT, async context =>
            {
                var candidate = _serializer.Deserialize<ConnectivityCandidate>(context.InputStream);
                var connection = _connections.GetConnection(candidate.ListeningPeer);
                var sessionId = P2PSessionId.From(candidate.SessionId);
                if (connection != null && connection.GetConnectionState().State == ConnectionState.Connected)
                {
                    _logger.Log(LogLevel.Warn, "p2p", $"Connection with {candidate.ListeningPeer} already existing and connected");
                    _sessions.UpdateSessionState(sessionId, P2PSessionState.Connected);
                    context.Send(stream =>
                    {
                        _serializer.Serialize(true, stream);
                    });
                    return;
                }

                _logger.Log(LogLevel.Debug, "p2p", "Connecting...");
                _ =_connections.AddPendingConnection(candidate.ListeningPeer).ContinueWith(task =>
                {
                    try
                    {
                        connection = task.Result;
                        connection.Metadata["type"] = "p2p";
                        _sessions.UpdateSessionState(sessionId, P2PSessionState.Connected);
                    }
                    catch (System.Exception ex)
                    {
                        _logger.Log(LogLevel.Error, "p2p.connect_client", $"An error occurred : {ex.Message}");
                    }
                });
                try
                {
                    connection = await _transport.Connect(candidate.ListeningEndpointCandidate.Address, sessionId.ToString(), context.Packet.Connection.Key);
                    _logger.Log(LogLevel.Debug, "p2p", $"Successfully connected to {connection.ToString()}");
                    context.Send(stream =>
                    {
                        bool connected = connection != null && connection.GetConnectionState().State == ConnectionState.Connected; 
                        _serializer.Serialize(connected, stream);
                    });
                }
                catch (System.Exception ex)
                {
                    _logger.Log(LogLevel.Error, "p2p", "Connection attempt failed : " + ex.Message, ex);
                    throw;
                }

            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_OPEN_TUNNEL, context =>
           {
               var serverId = _serializer.Deserialize<string>(context.InputStream);

               var peerId = context.Packet.Connection.Id;
               if (!_config.HasPublicIp)
               {
                   var handle = _tunnels.AddClient(serverId, peerId);
                   context.Send(stream =>
                   {
                       OpenTunnelResult result = new OpenTunnelResult();
                       result.UseTunnel = true;
                       result.Handle = handle;
                       _serializer.Serialize(result, stream);
                   });
               }
               else
               {
                   string endpoint = _config.IpPort;
                   context.Send(stream =>
                   {
                       OpenTunnelResult result = new OpenTunnelResult();
                       result.UseTunnel = false;
                       result.Endpoint = endpoint;
                       _serializer.Serialize(result, stream);
                   });
               }
               return Task.CompletedTask;
           });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_CLOSE_TUNNEL, context =>
            {
                var handle = _serializer.Deserialize<byte>(context.InputStream);
                _tunnels.CloseTunnel(handle, context.Packet.Connection.Id);
                return Task.CompletedTask;
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_RELAY_OPEN, context =>
            {
                var relay = _serializer.Deserialize<OpenRelayParameters>(context.InputStream);
                var sessionId = P2PSessionId.From(relay.SessionId);
                _connections.NewConnection(new RelayConnection(context.Packet.Connection, relay.RemotePeerAddress, relay.RemotePeerId, sessionId, _serializer));
                _sessions.UpdateSessionState(sessionId, P2PSessionState.Connected);
                context.Send(stream => { });
                return Task.CompletedTask;
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_RELAY_CLOSE, context =>
            {
                var peerId = _serializer.Deserialize<ulong>(context.InputStream);
                var connection = _connections.GetConnection(peerId);
                if (connection != null)
                {
                    _connections.CloseConnection(connection, "P2P relay closed");
                }
                context.Send(stream => { });
                return Task.CompletedTask;
            });

        }
    }
}