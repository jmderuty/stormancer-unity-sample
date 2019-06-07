using UnityEngine;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System;
using System.Threading.Tasks;
using Stormancer.Plugins;
using System.Linq;
using System.Threading;

namespace Stormancer
{

    public static class ClientProvider
    {
        private static ClientProviderImpl _instance;
        private static ClientProviderImpl Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ClientProviderImpl();
                }
                return _instance;
            }
        }


        public static Action<ClientConfiguration> OnClientConfiguration
        {
            get
            {
                return Instance.OnClientConfiguration;
            }
            set
            {
                Instance.OnClientConfiguration = value;
            }
        }

        public static Task<AuthParameters> OnRequestAuthParameters
        {
            get
            {
                return Instance.OnRequestAuthParameters;
            }
            set
            {
                Instance.OnRequestAuthParameters = value;
            }
        }

        public static T GetService<T>()
        {
            return Instance.GetService<T>();
        }

        public static void Clean()
        {
            Instance.CloseClient();
            _instance = null;
        }

        public static Networking.AddressType SocketAddressType
        {
            get
            {
                return Instance.SocketAddressType;
            }
        }

        public static Task<T> GetService<T>(string scene)
        {
            return Instance.GetService<T>(scene);
        }

        public static Task<Scene> GetPublicScene(string sceneid, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Instance.GetPublicScene(sceneid, cancellationToken);
        }

        public static Task<Scene> ConnectToPublicScene(string sceneId, Action<Scene> sceneInitializer)
        {
            return Instance.ConnectToPublicScene(sceneId, sceneInitializer);
        }

        public static Task<Scene> GetPrivateScene(string token)
        {
            return Instance.GetPrivateScene(token);
        }


        public static void DisconnectScene(string SceneId)
        {
            Instance.DisconnectScene(SceneId);
        }

        public static void ActivateAuthenticationPlugin()
        {
            Instance.AuthenticationPlugin = true;
        }

        public static void ActivateGameFinderPlugin()
        {
            Instance.GameFinderPlugin = true;
        }

        public static void ActivatePartyPlugin()
        {
            Instance.PartyPlugin = true;
        }

        public static void ActivateGameSessionPlugin()
        {
            Instance.GameSessionPlugin = true;
        }

        public static void ActivateLeaderboardPlugin()
        {
            Instance.LeaderboardPlugin = true;
        }
        public static void ActivateDebugLog()
        {
            Instance.DebugLog = true;
        }

        public static event Action<string> OnDisconnected
        {
            add
            {
                Instance.OnDisconnected += value;
            }
            remove
            {
                Instance.OnDisconnected -= value;
            }
        }

        public static long ClientId
        {
            get
            {
                return Instance.GetClientId();
            }
        }

        public static void SetAccountId(string str)
        {
            Instance.AccountId = str;
        }

        public static void SetApplicationName(string str)
        {
            Instance.ApplicationName = str;
        }

        public static void SetServerEndpoint(List<string> serverEndpoints)
        {
            Instance.ServerEndpoints = serverEndpoints;
        }

        public static void CloseClient()
        {
            Instance.CloseClient();
        }

        private class ClientProviderImpl
        {
            public string AccountId
            {
                get; set;
            }
            public string ApplicationName
            {
                get; set;
            }
            public List<string> ServerEndpoints
            {
                get; set;
            }
            public bool AuthenticationPlugin
            {
                get; set;
            }
            public bool DebugLog
            {
                get; set;
            }
            public bool GameFinderPlugin
            {
                get; set;
            }
            public bool PartyPlugin
            {
                get; set;
            }
            public bool GameSessionPlugin
            {
                get; set;
            }
            public bool LeaderboardPlugin
            {
                get; set;
            }


            public Action<ClientConfiguration> OnClientConfiguration { get; set; }

            public Task<AuthParameters> OnRequestAuthParameters { get; set; }

            private Action<string> _onDisconnected;
            private void InvokeDisonnected(string reason)
            {
                if (_onDisconnected != null)
                {
                    MainThread.Post(() =>
                    {
                        _onDisconnected?.Invoke(reason);
                    });
                }
            }

            public event Action<string> OnDisconnected
            {
                add
                {
                    _onDisconnected += value;
                }
                remove
                {
                    _onDisconnected -= value;
                }
            }

            private Client _client;
            private ConcurrentDictionary<string, Scene> _scenes = new ConcurrentDictionary<string, Scene>();

            /// <summary>
            /// return the stormancer client id if connected,
            /// otherwise return 0
            /// </summary>
            /// <returns></returns>
            public long GetClientId()
            {
                if (_client == null || _client.Id == null)
                {
                    return 0;
                }
                return _client.Id.Value;
            }

            public Networking.AddressType SocketAddressType
            {
                get
                {
                    return _client.AddressType;
                }
            }

            public async Task<Scene> GetPublicScene(string sceneId, CancellationToken cancellationToken = default(CancellationToken))
            {
                if (string.IsNullOrEmpty(sceneId))
                {
                    Debug.LogWarning("SceneID can't be empty, cannot connect to remote scene");
                    return null;
                }
                if (_client == null)
                {
                    InitClient();
                    if (_client == null)
                    {
                        return null;
                    }
                }
                if (_scenes.ContainsKey(sceneId))
                {
                    return _scenes[sceneId];
                }
                try
                {
                    var result = await _client.GetPublicScene(sceneId, cancellationToken);
                    Debug.Log("Retrieved remote scene");
                    _scenes.TryAdd(sceneId, result);
                    return result;
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                    return null;
                }
            }

            public async Task<Scene> GetPrivateScene(string token)
            {
                if (string.IsNullOrEmpty(token))
                {
                    Debug.LogWarning("token can't be empty, cannot connect to remote scene");
                    return null;
                }

                if (_client == null)
                {
                    InitClient();
                    if (_client == null)
                    {
                        return null;
                    }
                }
                try
                {
                    var result = await _client.GetPrivateScene(token);
                    Debug.Log("Retrieved remote scene");
                    _scenes.TryAdd(result.Id, result);
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex.InnerException);
                    return null;
                }
            }

            private void InitClient()
            {
                if (string.IsNullOrEmpty(AccountId) || string.IsNullOrEmpty(ApplicationName))
                {
                    Debug.LogError("AccountId or Application name are not set. Cannot connect to remoteScene");
                    return;
                }
                var config = ClientConfiguration.ForAccount(AccountId, ApplicationName);

                if (ServerEndpoints.Any())
                {
                    config.ServerEndpoints = ServerEndpoints;
                }

                if (AuthenticationPlugin)
                {
                    config.Plugins.Add(new AuthenticationPlugin());
                }
                if (GameFinderPlugin)
                {
                    config.Plugins.Add(new GameFinderPlugin());
                }
                if (GameSessionPlugin)
                {
                    config.Plugins.Add(new GameSessionPlugin());
                }
                if (LeaderboardPlugin)
                {
                    config.Plugins.Add(new LeaderboardPlugin());
                }
                if (PartyPlugin)
                {
                    config.Plugins.Add(new PartyPlugin());
                }

                if (DebugLog)
                {
                    config.Logger = DebugLogger.Instance;
                }

                config.TaskGetAuthParameters = OnRequestAuthParameters;

                OnClientConfiguration?.Invoke(config);

                _client = new Client(config);
                UniRx.MainThreadDispatcher.Initialize();
            }



            public void DisconnectScene(string sceneId)
            {
                Scene scene;

                if (_scenes.ContainsKey(sceneId) && _scenes.TryRemove(sceneId, out scene) == true)
                {
                    _ = scene.Disconnect();
                }
            }
             
            public void CloseClient()
            {
                using (_client)
                {
                    _scenes.Clear();
                }
            }

            public T GetService<T>()
            {
                if (_client == null)
                {
                    InitClient();
                    if (_client == null)
                    {
                        return default(T);
                    }
                }

                T result;
                if (!_client.DependencyResolver.TryResolve<T>(out result))
                {
                    return default(T);
                }

                return result;
            }


            //private readonly ConcurrentDictionary<string, Task<Scene>> _sceneTasks = new ConcurrentDictionary<string, Task<Scene>>();
            /// <summary>
            /// try to get a service on a specific private scene or an already connected public scene.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="sceneId"></param>
            /// <returns>return the service if is on scene, otherwise return null</returns>
            internal async Task<T> GetService<T>(string sceneId)
            {
                if (_scenes.ContainsKey(sceneId))
                {
                    return await TaskHelper.FromResult<T>( _scenes[sceneId].DependencyResolver.Resolve<T>());
                }

                var scene = await GetAndConnectScene(sceneId);
                _scenes.TryAdd(sceneId, scene);
                return scene.DependencyResolver.Resolve<T>();
            }

            private async Task<Scene> GetAndConnectScene(string sceneId)
            {
                var authenticationService = GetService<AuthenticationService>();
                if (authenticationService != null)
                {
                    return await authenticationService.ConnectToPrivateScene(sceneId, null); 
                }
                else
                {
                    return await ConnectToPublicScene(sceneId, null);
                }

            }

            internal async Task<Scene> ConnectToPublicScene(string sceneId, Action<Scene> sceneInitializer, CancellationToken cancellationToken = default(CancellationToken))
            {
                var scene = await GetPublicScene(sceneId, cancellationToken);
                if(!scene.Connected)
                {
                    sceneInitializer?.Invoke(scene);

                    try
                    {
                        await scene.Connect();
                    }
                    catch (Exception)
                    {
                        Scene sceneToDelete;
                        _scenes.TryRemove(sceneId, out sceneToDelete);
                        throw;
                    }
                }
                return scene;
            }
        }
    }
}