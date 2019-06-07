using System.Threading.Tasks;

namespace Stormancer.Plugins
{
    public class LeaderboardService 
    {
        private Scene _scene;

        public LeaderboardService(Scene scene)
        {
            _scene = scene;
        }

        public async Task<LeaderboardResult> Query(LeaderboardQuery query)
        {
            return await _scene.RpcTask<LeaderboardQuery, LeaderboardResult>("leaderboard.query", query);
        }

        public async Task<LeaderboardResult> Query(string cursor)
        {
            return await _scene.RpcTask<string, LeaderboardResult>("leaderboard.cursor", cursor);
        }
    }
}
