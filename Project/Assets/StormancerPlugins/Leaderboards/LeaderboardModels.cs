using MsgPack.Serialization;
using System;
using System.Collections.Generic;

namespace Stormancer.Plugins
{
    public enum ComparisonOperator : byte
    {
        GREATER_THAN_OR_EQUAL = 0,
        GREATER_THAN = 1,
        LESSER_THAN_OR_EQUAL = 2,
        LESSER_THAN = 3
    }

    public enum LeaderboardOrdering : byte
    {
        Ascending = 0,
        Descending = 1
    }

    [MsgPackDto]
    public class ScoreFilter
    {
        [MessagePackMember(0)]
        public ComparisonOperator Type { get; set; }

        [MessagePackMember(1)]
        public int Value { get; set; }
    }

    [MsgPackDto]
    public class FieldFilter
    {
        [MessagePackMember(0)]
        public string Field { get; set; }

        [MessagePackMember(1)]
        public List<string> Values { get; set; }
    }

    [MsgPackDto]
    public class LeaderboardQuery
    {
        [MessagePackMember(0)]
        public string StartId { get; set; }

        [MessagePackMember(1)]
        public List<ScoreFilter> ScoreFilters { get; set; }

        [MessagePackMember(2)]
        public List<FieldFilter> FieldFilters { get; set; }

        [MessagePackMember(3)]
        public int Size { get; set; } = 1;

        [MessagePackMember(4)]
        public int Skip { get; set; } = 0;

        [MessagePackMember(5)]
        public string LeaderboardName { get; set; }

        [MessagePackMember(6)]
        public List<string> FriendsIds { get; set; } = new List<string>();

        [MessagePackMember(7)]
        public LeaderboardOrdering Order { get; set; } = LeaderboardOrdering.Descending;
    }

    [MsgPackDto]
    public class ScoreRecord
    {
        [MessagePackMember(0)]
        public string Id { get; set; }

        [MessagePackMember(1)]
        public int Score { get; set; } = 0;

        [MessagePackMember(2)]
        public long CreatedOn { get; set; } = 0;

        [MessagePackMember(3)]
        public string Document { get; set; }
    }

    [MsgPackDto]
    public class LeaderboardRanking
    {
        [MessagePackMember(0)]
        public int Ranking { get; set; }

        [MessagePackMember(1)]
        public ScoreRecord ScoreRecord { get; set; }
    }

    [MsgPackDto]
    public class LeaderboardResult
    {
        [MessagePackMember(0)]
        public string LeaderboardName { get; set; } = "";

        [MessagePackMember(1)]
        public List<LeaderboardRanking> Results { get; set; } = new List<LeaderboardRanking>();

        [MessagePackMember(2)]
        public string Next { get; set; } = "";

        [MessagePackMember(3)]
        public string Previous { get; set; } = "";

        [MessagePackMember(4)]
        public long Total { get; set; }
    }
}