using MsgPack.Serialization;
using System.Collections.Generic;

namespace Stormancer.Plugins
{
    public enum ComparisonOperator
    {
        GREATER_THAN_OR_EQUAL = 0,
        GREATER_THAN = 1,
        LESSER_THAN_OR_EQUAL = 2,
        LESSER_THAN = 3
    }

    public enum LeaderboardOrdering
    {
        Ascending = 0,
        Descending = 1
    }

    [MsgPackDto]
    public class ScoreFilter
    {
        [MessagePackMember(0)]
        public ComparisonOperator Type { get; }

        [MessagePackMember(0)]
        public int Value { get; }
    }

    [MsgPackDto]
    public class FieldFilter
    {
        [MessagePackMember(0)]
        public string Field { get; }

        [MessagePackMember(1)]
        public List<string> Values { get; }
    }

    [MsgPackDto]
    public class LeaderboardQuery
    {
        [MessagePackMember(0)]
        public string StartId { get; }

        [MessagePackMember(1)]
        public List<ScoreFilter> ScoreFilters { get; } = new List<ScoreFilter>();

        [MessagePackMember(2)]
        public List<FieldFilter> FieldFilters { get; } = new List<FieldFilter>();

        [MessagePackMember(3)]
        public int Size { get; } = 1;

        [MessagePackMember(4)]
        public int Skip { get; } = 0;

        [MessagePackMember(5)]
        public string LeaderboardName { get; }

        [MessagePackMember(6)]
        public List<string> FriendsIds { get; } = new List<string>();

        [MessagePackMember(7)]
        public LeaderboardOrdering Order { get; } = LeaderboardOrdering.Descending;
    }

    [MsgPackDto]
    public class ScoreRecord
    {
        [MessagePackMember(0)]
        public string Id { get; }

        [MessagePackMember(1)]
        public int Score { get; } = 0;

        [MessagePackMember(2)]
        public long CreatedOn { get; } = 0;

        [MessagePackMember(3)]
        public string Document { get; }
    }

    [MsgPackDto]
    public class LeaderboardRanking
    {
        [MessagePackMember(0)]
        public int Ranking { get; } = 0;

        [MessagePackMember(1)]
        public ScoreRecord ScoreRecord { get; }
    }

    [MsgPackDto]
    public class LeaderboardResult
    {
        [MessagePackMember(0)]
        public List<LeaderboardRanking> Results { get; } = new List<LeaderboardRanking>();

        [MessagePackMember(1)]
        public string Next { get; }

        [MessagePackMember(2)]
        public string Previous { get; }
    }
}