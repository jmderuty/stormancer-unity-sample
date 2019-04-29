
namespace Stormancer.Plugins
{
    public class PartyContainer
    {
        private Scene _partyScene;
        public Scene PartyScene => _partyScene;

        public PartyContainer(Scene scene)
        {
            _partyScene = scene;
        }

        public bool IsSettingsValid => (_partyScene != null && _partyScene.DependencyResolver.Resolve<PartyService>() != null);
        public PartySettings Settings => _partyScene.DependencyResolver.Resolve<PartyService>().Settings;
        public PartyUserDto[] Members => _partyScene.DependencyResolver.Resolve<PartyService>().Members;
        public bool IsLeader => Settings.LeaderId == _partyScene.DependencyResolver.Resolve<AuthenticationService>().UserId;
        public string Id => _partyScene.Id;
    }
}