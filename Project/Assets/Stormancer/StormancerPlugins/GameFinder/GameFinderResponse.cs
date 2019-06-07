// :!Serialize
using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Plugins
{


    public class GameFinderResponse
    {
        public string ConnectionToken { get; set; }
        public Dictionary<string, string> OptionalParameters { get; set; } = new Dictionary<string, string>();
    }


    [MsgPackDto]
    public class GameFinderResponseDTO
    {

        [MessagePackMember(0)]
        public string GameToken { get; set; }

        [MessagePackMember(1)]
        public Dictionary<string, string> OptionalParameters { get; set; } = new Dictionary<string, string>();

    }
}