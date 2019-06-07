using System.Linq;


namespace Stormancer.Plugins
{
    public static class GameFinderExtensions
    {
        public static ReadyVerificationRequest ToModel(this ReadyVerificationRequestDto dto)
        {
            return new ReadyVerificationRequest
            {
                MatchId = dto.MatchId,
                Timeout = dto.Timeout,
                Members = dto.Members.ToDictionary(kvp => kvp.Key, kvp => (Readiness)kvp.Value)
            };
        }
    }
}
