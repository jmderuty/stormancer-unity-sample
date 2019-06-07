namespace Stormancer.Plugins
{

    public class LeaderboardPlugin : IClientPlugin
    {
        public void Build(PluginBuildContext ctx)
        {
            ctx.ClientCreated += ClientCreated;
            ctx.SceneCreated += SceneCreated;
        }

        public void SceneCreated(Scene scene)
        {
            if(scene.GetHostMetadata("stormancer.leaderboard") != null)
            {
                var service = new LeaderboardService(scene);
                scene.DependencyResolver.RegisterDependency<LeaderboardService>(service);
            }
        
        }

        public void ClientCreated(Client client)
        {
            client.DependencyResolver.Register<Leaderboard>(resolver => new Leaderboard(resolver.Resolve<AuthenticationService>()), true);
        }
    }
}
