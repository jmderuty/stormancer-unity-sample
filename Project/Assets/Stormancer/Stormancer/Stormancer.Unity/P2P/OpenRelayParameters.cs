using MsgPack.Serialization;

namespace Stormancer
{
    [MsgPackDto]
    public class OpenRelayParameters
    {
        [MessagePackMember(0)]
        public byte[] SessionId { get; set; }

        [MessagePackMember(1)]
        public ulong RemotePeerId { get; set; }

        [MessagePackMember(2)]
        public string RemotePeerAddress{ get; set; }
    }
}
