using System.Collections.Generic;

namespace Stormancer.Plugins
{
    public class ReadyVerificationRequest
    {
        public Dictionary<string, Readiness> Members { get; set; }
        public string MatchId { get; set; }
        public int Timeout { get; set; }
        public int MembersCountReady { get; set; }
        public int MembersCountTotal { get; set; }
    }
}