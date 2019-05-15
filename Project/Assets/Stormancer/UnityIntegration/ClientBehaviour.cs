using UnityEngine;
using System.Collections.Generic;
using Stormancer.UnityIntegration;
using System.Linq;
using Stormancer.Plugins;

namespace Stormancer
{
    public class ClientBehaviour : MonoBehaviour
    {
        private string _accountId;
        private string _applicationName;

        private List<string> _serverEndpoints = new List<string>();

        private bool _debugLog = true;
        private bool _localServer = false; 

        public StringEvent OnDisconnected;

        private IAuthenticationProvider _provider;

        public void Awake()
        {
            if(_localServer)
            {
                _accountId = "unity";
                _applicationName = "dev";
                _serverEndpoints.Add("http://192.168.2.103");
            }
            else
            {
                _accountId = "virtual-regatta";
                _applicationName = "dev-server";
                _serverEndpoints.Add("http://gc3.stormancer.com");
            }
            DontDestroyOnLoad(gameObject);
            _provider = new RandomAuthenticationProvider();
            Initialize();
        }


        public void Initialize()
        {
            ClientProvider.SetAccountId(_accountId);
            ClientProvider.SetApplicationName(_applicationName);
            _provider.Initialize();
            ClientProvider.OnRequestAuthParameters = _provider.GetAuthArgs();

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
