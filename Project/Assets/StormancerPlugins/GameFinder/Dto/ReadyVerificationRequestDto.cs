// :!Serialize
using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Plugins
{
    [MsgPackDto]
    public class ReadyVerificationRequestDto
    {
        [MessagePackMember(0)]
        public Dictionary<string, int> Members { get; set; }

        [MessagePackMember(1)]
        public string MatchId { get; set; }

        [MessagePackMember(2)]
        public int Timeout { get; set; }

        [MessagePackMember(3)]
        public int MembersCountReady { get; set; }

        [MessagePackMember(4)]
        public int MembersCountTotal { get; set; }

    }
}