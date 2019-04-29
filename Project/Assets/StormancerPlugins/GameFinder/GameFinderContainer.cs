using System;

namespace Stormancer.Plugins
{

    public class GameFinderContainer
    {
        public Scene Scene { get; set; }
        public GameFinderService Service
        {
            get
            {
                return Scene.DependencyResolver.Resolve<GameFinderService>();
            }
        }
    }

}
