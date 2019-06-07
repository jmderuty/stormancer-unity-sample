
using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Plugins
{

    [MsgPackDto]
    public class SetResult
    {

        [MessagePackMember(0)]
        public string WinnerId { get; set; }

        [MessagePackMember(1)]
        public int WinnerScore { get; set; }

        [MessagePackMember(2)]
        public int LoserScore { get; set; }

    }

    [MsgPackDto]
    public class GameResult
    {

        [MessagePackMember(0)]
        public string WinnerId { get; set; }

        [MessagePackMember(1)]
        public bool Forfait { get; set; }

        [MessagePackMember(2)]
        public List<SetResult> Sets { get; set; }

    }

    [MsgPackDto]
    public class EndGameDto
    {

        [MessagePackMember(0)]
        public long Score { get; set; }

        [MessagePackMember(1)]
        public string LeaderboardName { get; set; }

    }
    [MsgPackDto]
    public class GameSessionResult
    {
        [MessagePackMember(0)]
        public Dictionary<string, string> UsersScore { get; set; }
    }


}
