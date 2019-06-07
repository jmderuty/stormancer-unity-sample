using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Plugins
{
    [MsgPackDto]
    public class AuthParameters
    {
        [MessagePackMember(0)]
        public string Type { get; set; }

        [MessagePackMember(1)]
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
