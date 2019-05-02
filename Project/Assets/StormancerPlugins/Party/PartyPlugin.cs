namespace Stormancer.Plugins
{
    public class PartyPlugin : IClientPlugin
    {
        public void Build(PluginBuildContext ctx)
        {
            ctx.SceneCreated += SceneCreated;
            ctx.ClientCreated += ClientCreated;
        }

        public void SceneCreated(Scene scene)
        {
            if(scene != null)
            {
                var name = scene.GetHostMetadata("stormancer.party");
                if(name != null)
                {
                    var service = new PartyService(scene);
                    service.Initialize();
                    scene.DependencyResolver.RegisterDependency<PartyService>(service);
                }

                name = scene.GetHostMetadata("stormancer.partymanagement");
                if(name != null)
                {
                    var service = new PartyManagementService(scene);
                    scene.DependencyResolver.RegisterDependency(service);
                }
            }
        }

        public void ClientCreated(Client client)
        {
            if(client != null)
            {
                client.DependencyResolver.Register((resolver) =>
                {
                    var service = new Party(resolver.Resolve<AuthenticationService>(), resolver.Resolve<ILogger>(), resolver.Resolve<ISerializer>(), resolver.Resolve<GameFinder>());
                    service.Initialize();
                    return service;
                }, true);
            }
        }
    }
}
