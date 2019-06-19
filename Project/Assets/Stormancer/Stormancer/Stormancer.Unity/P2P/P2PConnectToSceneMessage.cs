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
        public Dictionary<string, string> SceneMetadata { get; set; } = new Dictionary<string, string>();

        [MessagePackMember(3)]
        public Dictionary<string, string> ConnectionMetadata { get; set; } = new Dictionary<string, string>();

        [MessagePackMember(4)]
        public List<RouteDto> Routes { get; set; } = new List<RouteDto>();
    }
}