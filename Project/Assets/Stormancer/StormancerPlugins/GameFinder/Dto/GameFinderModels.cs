using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Plugins
{

    public class GameFinderStatusChangedEvent
    {
        public GameFinderStatus Status { get; set; }
        public string GameFinder { get; set; }
    }

    public class GameFoundEvent
    {
        public GameFinderResponse Data { get; set; }
        public string GameFinder { get; set; }
    }

    public class FindGameFailedEvent
    {
        public string Reason { get; set; }
        public string GameFinder { get; set; }
    }

    [MsgPackDto]
    public class GameFinderRequest
    {
        [MessagePackMember(0)]
        public Dictionary<string, string> ProfileIds { get; set; } = new Dictionary<string, string>();
    }

    [MsgPackDto]
    public class PlayerDTO
    {
        [MessagePackMember(0)]
        public ulong SteamId { get; set; }

        [MessagePackMember(1)]
        public string PlayerId { get; set; }

        [MessagePackMember(2)]
        public string SteamName { get; set; }

    }

    [MsgPackDto]
    public class TeamDTO
    {
        [MessagePackMember(0)]
        public List<PlayerDTO> Team { get; set; } = new List<PlayerDTO>();

    }

    [MsgPackDto]
    public class PlayerProfile
    {

        [MessagePackMember(0)]
        public string Id { get; set; }
        [MessagePackMember(1)]
        public string PlayerId { get; set; }
    }

    [MsgPackDto]
    public class ProfileSummary
    {
        [MessagePackMember(0)]
        public string ProfileName { get; set; }
        [MessagePackMember(1)]
        public string Id { get; set; }
    }

}