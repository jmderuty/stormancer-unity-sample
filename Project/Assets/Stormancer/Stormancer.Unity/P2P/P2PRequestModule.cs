using Stormancer.Client45;
using Stormancer.Networking;
using System.Collections.Generic;
using System;
using System.Text;

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
            builder.Service((byte)SystemRequestIDTypes.ID_P2P_GATHER_IP, async context =>
            {
                List<string> endpoints = new List<string>();
                if(_config.DedicatedServerEndpoint != "")
                {
                    endpoints.Add(_config.DedicatedServerEndpoint + ":" + _config.ClientSDKPort);
                }
                else if(!_config.EnableNatPunchthrough)
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
            });

            builder.Service((byte)SystemRequestIDTypes.ID_DISCONNECT_FROM_SCENE, async context =>
            {
                string sceneId = _serializer.Deserialize<string>(context.InputStream);
                string reason = _serializer.Deserialize<string>(context.InputStream);

                await _client.Disconnect(sceneId, true, reason);

            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_CREATE_SESSION, async context =>
            {
                var sessionId = _serializer.Deserialize<byte[]>(context.InputStream);
                var peerId = _serializer.Deserialize<ulong>(context.InputStream);
                var sceneId = _serializer.Deserialize<string>(context.InputStream);
                P2PSession session = new P2PSession();
                session.SessionId = sessionId;
                session.SceneId = sceneId;
                session.RemotePeer = peerId;
                _sessions.CreateSession(sessionId, session);
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_CLOSE_SESSION, async context =>
            {
                var sessionId = _serializer.Deserialize<byte[]>(context.InputStream);
                _sessions.CloseSession(sessionId);
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_TEST_CONNECTIVITY_CLIENT, async context =>
            {
                throw new NotImplementedException();
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_TEST_CONNECTIVITY_HOST, async context =>
            {
                throw new NotImplementedException();
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_CONNECT_HOST, async context =>
            {
                throw new NotImplementedException();
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_CONNECT_CLIENT, async context =>
            {
                throw new NotImplementedException();
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_OPEN_TUNNEL, async context =>
            {
                throw new NotImplementedException();
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_CLOSE_TUNNEL, async context =>
            {
                throw new NotImplementedException();
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_RELAY_OPEN, async context =>
            {
                throw new NotImplementedException();
            });

            builder.Service((byte)SystemRequestIDTypes.ID_P2P_RELAY_CLOSE, async context =>
            {
                throw new NotImplementedException();
            });
        }
    }
}