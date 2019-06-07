namespace Stormancer.Plugins
{
    public class GameSessionPlugin : IClientPlugin
    {
        /// <summary>
        /// Register game session callbacks to the PluginBuildContext
        /// </summary>
        /// <param name="ctx"></param>
        public void Build(PluginBuildContext ctx)
        {
            ctx.ClientCreated += ClientCreated;
            ctx.SceneDisconnecting += SceneDisconnecting;
            ctx.SceneCreated += SceneCreated;
        }

        /// <summary>
        /// Callback to the creation of a scene. Register the GameSessionService in the scene dependency resolver
        /// </summary>
        /// <param name="scene">The scene that was created</param>
        private void SceneCreated(Scene scene)
        {
            if (scene != null)
            {
                var name = scene.GetHostMetadata("stormancer.gamesession");
                if (name != null)
                {
                    var service = new GameSessionService(scene);
                    service.Initialize();
                    scene.DependencyResolver.RegisterDependency(service);
                }
            }
        }

        /// <summary>
        /// Callback to the scene Disconnection. Call the disconnection of the GameSessionService
        /// </summary>
        /// <param name="scene">The scene that is disconnecting</param>
        private void SceneDisconnecting(Scene scene)
        {
            if (scene != null)
            {
                var name = scene.GetHostMetadata("stormancer.gamesession");
                if(name != null)
                {
                    var gameSession = scene.DependencyResolver.Resolve<GameSessionService>();
                    gameSession.OnDisconnecting();
                }
            }
        }

        /// <summary>
        /// Callback to the creation of a client. Register a GameSession in the client dependency resolver
        /// </summary>
        /// <param name="client">The newly created client</param>
        private void ClientCreated(Client client)
        {
            if(client != null)
            {
                client.DependencyResolver.RegisterDependency(new GameSession(client));
            }
        }

    }
}