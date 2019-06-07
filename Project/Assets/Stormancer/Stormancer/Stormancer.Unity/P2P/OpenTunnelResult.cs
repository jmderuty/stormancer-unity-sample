using MsgPack.Serialization;

namespace Stormancer
{
    [MsgPackDto]
    public class OpenTunnelResult
    {
        [MessagePackMember(0)]
        public bool UseTunnel { get; set; }

        [MessagePackMember(1)]
        public string Endpoint { get; set; }

        [MessagePackMember(2)]
        public byte Handle { get; set; }

    }
}