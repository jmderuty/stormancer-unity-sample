using MsgPack.Serialization;

namespace Stormancer.Plugins
{
    [MsgPackDto]
    public class ServerStartedMessage
    {
        [MessagePackMember(0)]
        public string P2PToken { get; set; }
    }

    public class GameSessionConnectionParameters
    {
        public bool IsHost { get; set; }
        public string HostMap { get; set; }
        public string Endpoint { get; set; }
        public string ErrorMessage { get; set; }
    }

}