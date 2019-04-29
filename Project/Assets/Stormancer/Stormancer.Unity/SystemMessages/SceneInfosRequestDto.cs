// :!Serialize
using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Dto
{
    [MsgPackDto]
    public class SceneInfosRequestDto
    {
        public string Token;
        public Dictionary<string, string> Metadata;
    }
}
