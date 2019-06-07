// :!Serialize
using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Dto
{
    [MsgPackDto]
    public class ConnectToSceneMsg
    {
        public string Token;
        public List<RouteDto> Routes;
        public Dictionary<string, string> ConnectionMetadata { get; set; }
    }
}
