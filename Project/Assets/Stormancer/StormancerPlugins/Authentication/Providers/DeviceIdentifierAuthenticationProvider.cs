
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Stormancer.Plugins
{
    class DeviceIdentifierAuthenticationProvider : IAuthenticationProvider
    {

        private string _identifier;

        public void Initialize()
        {
            _identifier = UnityEngine.SystemInfo.deviceUniqueIdentifier;
            if (Application.isEditor)
            {
                _identifier += "_Editor";
            }
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
