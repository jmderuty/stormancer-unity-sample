using Stormancer.Networking;
using System.Threading.Tasks;

namespace Stormancer
{
    public class P2PPacketDispatcher : IPacketProcessor
    {
        private P2PTunnels _tunnels;
        private IConnectionManager _connections;
        private ILogger _logger;
        private ISerializer _serializer;

        public P2PPacketDispatcher(P2PTunnels tunnels, IConnectionManager connections, ILogger logger, ISerializer serializer)
        {
            _tunnels = tunnels;
            _connections = connections;
            _logger = logger;
            _serializer = serializer;
        }

        public void RegisterProcessor(PacketProcessorConfig config)
        {
            config.AddProcessor((byte)MessageIDTypes.ID_P2P_RELAY, packet =>
            { 
                if(_serializer == null || _connections == null)
                {
                    return Task.FromResult(false);
                }
                var peerId = _serializer.Deserialize<ulong>(packet.Stream);
                var connection = _connections.GetConnection(peerId);
                if(connection != null)
                {
                    packet.Connection = connection;
                }
                return Task.FromResult(false);
            });

            config.AddProcessor((byte)MessageIDTypes.ID_P2P_TUNNEL, packet =>
            {
                _tunnels.ReceivedFrom(packet.Connection.Id, packet.Stream);
                return Task.FromResult(true);
            });
        }
    }
}
