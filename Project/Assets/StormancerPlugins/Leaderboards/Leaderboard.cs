
using System.Threading.Tasks;

namespace Stormancer.Plugins
{
    public class Leaderboard : ClientAPI<Leaderboard>
    {
        public Leaderboard(AuthenticationService authenticationService) : base(authenticationService)
        {
        }

        public async Task<LeaderboardResult> Query(LeaderboardQuery query)
        {
            var service = await GetLeaderboardService();
            return await service.Query(query);
        }

        public async Task<LeaderboardResult> Query(string cursor)
        {
            var service = await GetLeaderboardService();
            return await service.Query(cursor);
        }

        private async Task<LeaderboardService> GetLeaderboardService()
        {
            return await GetService<LeaderboardService>("stormancer.leaderboard");
        }
    }
}
