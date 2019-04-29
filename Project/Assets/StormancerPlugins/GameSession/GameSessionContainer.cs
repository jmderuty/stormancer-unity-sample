using System.Threading.Tasks;

namespace Stormancer.Plugins
{
    public class GameSessionContainer
    {
        public Scene Scene { get; set; }
        public Task<GameSessionConnectionParameters> GameSessionReadyTask { get; set; }

        public GameSessionService Service
        {
            get => Scene?.DependencyResolver.Resolve<GameSessionService>();
        }

    }
}
