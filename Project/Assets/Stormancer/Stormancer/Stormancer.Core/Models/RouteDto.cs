// :!Serialize
using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Dto
{
    [MsgPackDto]
    public class RouteDto
    {
        [MessagePackMember(0)]
        public string Name { get; set; }

        [MessagePackMember(1)]
        public ushort Handle { get; set; }

        [MessagePackMember(2)]
        public Dictionary<string, string> Metadata { get; set; }
    }
}
