// :!Serialize
using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Dto
{
    [MsgPackDto]
    public class SceneInfosDto
    {
        [MessagePackMember(0)]
        public string SceneId;

        [MessagePackMember(1)]
        public Dictionary<string, string> Metadata;

        [MessagePackMember(2)]
        public List<RouteDto> Routes;

        [MessagePackMember(3)]
        public string SelectedSerializer;
    }
}
