using System.Collections.Generic;

namespace Stormancer.Plugins
{
    public enum GameFinderStatus
    {
        Idle = -1,

        Searching = 0,
        CandidateFound = 1,
        WaitingPlayersReady = 2,
        Success = 3,
        Failed = 4,
        Canceled = 5,
        Loading = 6
    }
}