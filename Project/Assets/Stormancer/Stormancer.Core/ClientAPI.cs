using Stormancer.Core;
using Stormancer.Plugins;
using System;
using System.Threading.Tasks;
using UniRx;

namespace Stormancer
{
    public abstract class ClientAPI<TManager>
    {
        private Scene _scene = null;
        protected Authentication _auth = null;
        private IDisposable _sceneStatusChangedSubscription;

        private Action<ClientAPI<TManager>, Scene> _cleanupCallback = null;

        public ClientAPI(Authentication authenticationService)
        {
            _auth = authenticationService;
        }

        protected async Task<TService> GetService<TService>(string type,
                                                    Action<ClientAPI<TManager>, TService, Scene> initializer = null,
                                                    Action<ClientAPI<TManager>, Scene> cleanup = null,
                                                    string name = "")
        {

            if (_scene == null)
            {
                var scene = await _auth.GetSceneForService(type, name);

                _cleanupCallback = cleanup;
                _sceneStatusChangedSubscription = scene.SceneConnectionStateObservable.Subscribe(OnSceneConnectionStateChanged);

                // not sure if it is mandatory in c#
                if (scene.GetCurrentConnectionState() == ConnectionState.Disconnected || scene.GetCurrentConnectionState() == ConnectionState.Disconnecting)
                {
                    cleanup?.Invoke(this, scene);
                    _sceneStatusChangedSubscription.Dispose();
                }
                _scene = scene;
            }

            var service = _scene.DependencyResolver.Resolve<TService>();
            initializer?.Invoke(this, service, _scene);
            return service;
        }

        private void OnSceneConnectionStateChanged(ConnectionStateCtx stateCtx)
        {
            ConnectionState state = stateCtx.State;

            if (state == ConnectionState.Disconnected || state == ConnectionState.Disconnecting)
            {
                _cleanupCallback(this, _scene);
                RemoveCallbackStateChanged(_scene);
                _scene = null;
            }
        }

        private void RemoveCallbackStateChanged(Scene scene)
        {
            _sceneStatusChangedSubscription.Dispose();
        }
    }
}
