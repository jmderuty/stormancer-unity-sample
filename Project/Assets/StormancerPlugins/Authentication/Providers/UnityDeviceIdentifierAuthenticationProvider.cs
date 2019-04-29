
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stormancer.Plugins
{
    class DeviceIdentifierAuthenticationProvider : IAuthenticationProvider
    {

        private string _identifier;

        public void Initialize()
        {
            _identifier = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            ClientProvider.ActivateAuthenticationPlugin();
            ClientProvider.ActivateGameSessionPlugin();
            ClientProvider.ActivateGameFinderPlugin();
        }

        public Task<AuthParameters> GetAuthArgs()
        {
            AuthParameters auth = new AuthParameters();
            auth.Type = "deviceidentifier";
            auth.Parameters = new Dictionary<string, string> { { "deviceidentifier", _identifier } };
            return Task.FromResult(auth);
        }
    }
}
