
namespace Stormancer.Plugins
{
    public class AuthenticationPlugin : IClientPlugin
    {
        public void Build(PluginBuildContext ctx)
        {
            ctx.ClientCreated += ClientCreated;
        }

        private void ClientCreated(Client client)
        {
            var authenticationService = new Authentication(client);
            client.DependencyResolver.RegisterDependency(authenticationService);
        }
    }
}
