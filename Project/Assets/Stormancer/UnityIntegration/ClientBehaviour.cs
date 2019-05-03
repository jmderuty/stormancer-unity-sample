using UnityEngine;
using System.Collections.Generic;
using Stormancer.UnityIntegration;
using System.Linq;
using Stormancer.Plugins;

namespace Stormancer
{
    public class ClientBehaviour : MonoBehaviour
    {
        private string AccountId = "virtual-regatta";
        private string ApplicationName = "dev-server";
        //private string AccountId = "unity";
        //private string ApplicationName = "dev";

        private List<string> ServerEndpoints = new List<string>();

        private bool DebugLog = true;
        
        public StringEvent OnDisconnected;

        private IAuthenticationProvider _provider;

        public void Awake()
        {
            ServerEndpoints.Add("http://gc3.stormancer.com");
            //ServerEndpoints.Add("http://192.168.2.103");
            DontDestroyOnLoad(gameObject);
            _provider = new RandomAuthenticationProvider();
            Initialize();
        }


        public void Initialize()
        {
            ClientProvider.SetAccountId(AccountId);
            ClientProvider.SetApplicationName(ApplicationName);
            _provider.Initialize();
            ClientProvider.OnRequestAuthParameters = _provider.GetAuthArgs();

            if (ServerEndpoints.Any())
            {
                ClientProvider.SetServerEndpoint(ServerEndpoints);
            }

            if (DebugLog)
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
