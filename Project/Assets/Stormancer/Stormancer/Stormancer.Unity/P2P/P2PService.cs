using Stormancer.Core;
using Stormancer.Networking;
using System;
using System.Threading;
using System.Threading.Tasks;
using Stormancer.Networking.Processors;
using Stormancer.Client45;
using Stormancer.Diagnostics;

namespace Stormancer
{
    public class P2PService
    {
        private readonly IConnectionManager _connections;
        private readonly ITransport _transport;
        private readonly ISerializer _serializer;
        private readonly RequestProcessor _sysCall;
        private readonly P2PTunnels _tunnels;
        private readonly ILogger _logger;

        public P2PService(IConnectionManager connections, RequestProcessor sysClient, ITransport transport, ISerializer serializer, P2PTunnels tunnels, ILogger logger)
        {
            _connections = connections;
            _sysCall = sysClient;
            _transport = transport;
            _serializer = serializer;
            _tunnels = tunnels;
            _logger = logger;
        }

        public P2PTunnel RegisterP2PServer(string p2pServerId)
        {
            return _tunnels.CreateServer(p2pServerId, _tunnels);
        }

        public async Task<P2PTunnel> OpenTunnel(ulong connectionId, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _tunnels.OpenTunnel(connectionId, id, cancellationToken);
        }

        public async Task<IConnection> OpenP2PConnection(IConnection server, string p2pToken, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(server == null)
            {
                throw new InvalidOperationException("Cannot establish P2P connection. The client must be connected to the server");
            }

            var session = await _sysCall.SendSystemRequest<P2PSession, string>(server, (byte)SystemRequestIDTypes.ID_P2P_CREATE_SESSION, p2pToken, cancellationToken);
            var connection = _connections.GetConnection(session.RemotePeer);
            if(connection == null)
            {
                throw new InvalidOperationException("Failed to get P2P connection for peer " + session.RemotePeer);
            }
            return connection;

        }
    }
}