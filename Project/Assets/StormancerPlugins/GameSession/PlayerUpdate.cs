// :!Serialize
using MsgPack.Serialization;

namespace Stormancer.Plugins
{
    [MsgPackDto]
    public class PlayerUpdate
    {
        [MessagePackMember(0)]
        public string UserId { get; set; }

        [MessagePackMember(1)]
        public int Status { get; set; }

        [MessagePackMember(2)]
        public string Data { get; set; }
    }
}