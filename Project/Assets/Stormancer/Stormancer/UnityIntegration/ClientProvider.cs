﻿using UnityEngine;
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

        /// <summary>
        /// Return the service of type T from the client
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <returns>The service of type T</returns>
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

        /// <summary>
        /// Return the service of type T from the scene
        /// </summary>
        /// <typeparam name="T">Type of the service</typeparam>
        /// <param name="scene">The id of the scene to get the service from</param>
        /// <returns>The service of type T</returns>
        public static Task<T> GetService<T>(string scene)
        {
            return Instance.GetService<T>(scene);
        }

        /// <summary>
        /// Return the public scene with the id sceneid
        /// </summary>
        /// <param name="sceneid">the id of the public scene to return</param>
        /// <param name="cancellationToken"></param>
        /// <returns>the public scene with the id sceneid</returns>
        public static Task<Scene> GetPublicScene(string sceneid, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Instance.GetPublicScene(sceneid, cancellationToken);
        }

        /// <summary>
        /// Connect and execute the sceneInitializer callback on the public scene with Id sceneId, then return the scene
        /// </summary>
        /// <param name="sceneId">the id of the public scene</param>
        /// <param name="sceneInitializer">Initialisation callback</param>
        /// <returns>the connected and initialized public scene with id sceneId</returns>
        public static Task<Scene> ConnectToPublicScene(string sceneId, Action<Scene> sceneInitializer)
        {
            return Instance.ConnectToPublicScene(sceneId, sceneInitializer);
        }

        /// <summary>
        /// Return a private scene from it's connection token
        /// </summary>
        /// <param name="token">Connection token of the private scene</param>
        /// <returns>the private scene</returns>
        public static Task<Scene> GetPrivateScene(string token)
        {
            return Instance.GetPrivateScene(token);
        }


        public static void DisconnectScene(string SceneId)
        {
            Instance.DisconnectScene(SceneId);
        }

        /// <summary>
        /// Configuration method to activate stormancer log to be displayed in unity
        /// </summary>
        public static void ActivateDebugLog()
        {
            Instance.UseDebug = true;
        }

        /// <summary>
        /// Configuration method to add a stormancer plugin
        /// </summary>
        /// <param name="plugin">Stormancer plugin to add</param>
        public static void AddPlugin(IClientPlugin plugin)
        {
            Instance.Plugins.Add(plugin);
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
            public List<IClientPlugin> Plugins {get; set;} = new List<IClientPlugin>();
            public bool UseDebug { get; set; } = false;

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
                config.Plugins.AddRange(Plugins);
                if(UseDebug)
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