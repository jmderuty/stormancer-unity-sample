using UnityEngine;
using System.Collections.Generic;
using Stormancer.UnityIntegration;
using System.Linq;
using Stormancer.Plugins;
using System;
using System.Threading.Tasks;

namespace Stormancer
{
    public class ClientBehaviour : MonoBehaviour
    {
#error TODO: Setup accountId, applicationName and endpoints.
        private string _accountId = "sample-unity";
        private string _applicationName = "sample";

        private List<string> _serverEndpoints = new List<string>() { "http://gc3.stormancer.com" };

        private IAuthenticationProvider _authenticationProvider;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
#error TODO: Select an authenticationProvider and Add the plugin you'll need
            _authenticationProvider = new RandomAuthenticationProvider();
            ClientProvider.AddPlugin(new AuthenticationPlugin());
            ClientProvider.AddPlugin(new GameSessionPlugin());
            ClientProvider.AddPlugin(new GameFinderPlugin());
            ClientProvider.AddPlugin(new PartyPlugin());
            ClientProvider.AddPlugin(new LeaderboardPlugin());
            ClientProvider.ActivateDebugLog();
            Initialize();
        }


        public void Initialize()
        {
            ClientProvider.SetAccountId(_accountId);
            ClientProvider.SetApplicationName(_applicationName);
            _authenticationProvider.Initialize();
            ClientProvider.OnRequestAuthParameters = _authenticationProvider.GetAuthArgs();

            if (_serverEndpoints.Any())
            {
                ClientProvider.SetServerEndpoint(_serverEndpoints);
            }
        }
    }
}
