using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Stormancer;
using Stormancer.Core;
using System;
using System.Threading.Tasks;

namespace Stormancer
{
    public class RemoteScene : MonoBehaviour
    {
        public string SceneId = "";
        public bool ConnectOnLoad = true;
        public bool DisconnectOnUnLoad = true;
        public bool Connected = false;

        private List<RemoteLogicBase> _localLogics = new List<RemoteLogicBase>();
        public List<RemoteLogicBase> LocalLogics
        {
            get
            {
                return _localLogics;
            }
        }

        public Scene Scene { get; private set; }

        void Start()
        {
            if (ConnectOnLoad)
            {
                ConnectScene();
            }
        }

        public async void ConnectScene()
        {
            var scene = await ClientProvider.GetPublicScene(SceneId);
            await InitSceneAndConnect(scene);
        }

        public async void ConnectPrivateScene(string token)
        {
            var scene = await ClientProvider.GetPrivateScene(token);
            await InitSceneAndConnect(scene);
        }

        public async Task<Scene> ConnectPrivateSceneTask(string token)
        {
            var scene = await ClientProvider.GetPrivateScene(token);
            return await InitSceneAndConnect(scene);
        }

        private async Task<Scene> InitSceneAndConnect(Scene scene)
        {
            if (scene != null)
            {
                Scene = scene;
                SceneId = scene.Id;
                foreach (RemoteLogicBase logic in LocalLogics)
                {
                    logic.Init(Scene);
                }
                try
                {
                    await Scene.Connect();
                    if (Scene.Connected)
                    {
                        Debug.Log("connected to scene: " + SceneId);
                        Connected = true;
                        foreach (RemoteLogicBase remotelogic in LocalLogics)
                        {
                            remotelogic.OnConnected();
                        }
                        return Scene;
                    }
                    else
                    {
                        Debug.LogWarning("failed to connect to scene: " + SceneId);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("failed to connect to scene: " + SceneId + " : " + ex.Message);
                }
            }
            return null;
        }

        void Disconnect()
        {
            if (Scene != null && Scene.Connected)
            {
                ClientProvider.DisconnectScene(SceneId);
            }
        }
        

        void OnDestroy()
        {
            if (DisconnectOnUnLoad)
            {
                Disconnect();
            }
        }

        void OnApplicationQuit()
        {
            Disconnect();
            ClientProvider.CloseClient();
        }
    }
}
