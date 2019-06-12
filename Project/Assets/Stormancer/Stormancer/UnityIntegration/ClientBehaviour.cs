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
#warning TODO: Setup accountId, applicationName and endpoints.
        private string _accountId = "unity";
        private string _applicationName = "dev";

        private List<string> _serverEndpoints = new List<string>() { "http://127.0.0.1" };

        private bool _debugLog = true;

        public StringEvent OnDisconnected;

        private IAuthenticationProvider _authenticationProvider;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _authenticationProvider = new RandomAuthenticationProvider();
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

            if (_debugLog)
            {
                ClientProvider.ActivateDebugLog();
            }

            if (OnDisconnected != null)
            {
                ClientProvider.OnDisconnected += (s => MainThread.Post(() => OnDisconnected.Invoke(s)));
            }
        }
    }
}
