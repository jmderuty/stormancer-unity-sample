using MsgPack.Serialization;

namespace Stormancer
{
    [MsgPackDto]
    public class ConnectivityCandidate
    {
        [MessagePackMember(0)]
        public ulong ListeningPeer { get; set; }

        [MessagePackMember(1)]
        public ulong ClientPeer { get; set; }

        [MessagePackMember(2)]
        public EndpointCandidate ListeningEndpointCandidate { get; set; }

        [MessagePackMember(3)]
        public EndpointCandidate ClientEndpointCandidate { get; set; }

        [MessagePackMember(4)]
        public byte[] SessionId { get; set; }

        [MessagePackMember(5)]
        public byte[] Id { get; set; }

        [MessagePackMember(6)]
        public int Ping { get; set; }
    }
}
