using MsgPack.Serialization;
using Stormancer.Dto;
using System.Collections.Generic;

namespace Stormancer
{
    [MsgPackDto]
    public class P2PConnectToSceneMessage
    {
        [MessagePackMember(0)]
        public string SceneId { get; set; }

        [MessagePackMember(1)]
        public byte SceneHandle { get; set; }

        [MessagePackMember(2)]
        public Dictionary<string, string> Metadata { get; set; }

        [MessagePackMember(3)]
        public List<RouteDto> Routes { get; set; }
    }
}