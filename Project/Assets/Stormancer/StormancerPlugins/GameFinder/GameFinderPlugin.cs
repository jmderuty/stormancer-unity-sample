
namespace Stormancer.Plugins
{
    public class GameFinderPlugin : IClientPlugin
    {
        public void Build(PluginBuildContext ctx)
        {
            ctx.SceneCreated += SceneCreated;
            ctx.ClientCreated += ClientCreated;
        }

        private void SceneCreated(Scene scene)
        {
            var pluginName = scene.GetHostMetadata("stormancer.plugins.gamefinder");
            if(!string.IsNullOrEmpty(pluginName))
            {
                var gameFinderService = new GameFinderService(scene);
                gameFinderService.Initialize();
                scene.DependencyResolver.RegisterDependency(gameFinderService);
            }
        }

        private void ClientCreated(Client client)
        {
            client.DependencyResolver.RegisterDependency<GameFinder>(new GameFinder(client.DependencyResolver.Resolve<AuthenticationService>(), client.DependencyResolver.Resolve<ILogger>()));
        }
    }
}
