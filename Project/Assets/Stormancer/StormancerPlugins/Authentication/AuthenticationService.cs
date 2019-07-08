using Stormancer.Core;
using Stormancer.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;

namespace Stormancer.Plugins
{

    public class AuthenticationService
    {
        private readonly Client _client;
        private readonly ILogger _logger;
        private readonly string SCENE_ID = "authenticator";
        private readonly string LOGIN_ROUTE = "Authentication.Login";

        private bool _autoReconnect = false;
        private Task<Scene> _authTask = null;

        public string UserId { get; private set; }
        public string UserName { get; private set; }

        private GameConnectionStateCtx _gameConnectionStateCtx = new GameConnectionStateCtx(GameConnectionState.Disconnected);
        private Dictionary<string, Func<OperationCtx, Task>> _operationHandlers = new Dictionary<string, Func<OperationCtx, Task>>();

        public GameConnectionState State
        {
            get
            {
                return _gameConnectionStateCtx.State;
            }
            private set
            {
                _gameConnectionStateCtx.State = value;
            }
        }

        public Func<Task<AuthParameters>> OnGetAuthParameters { get; set; }

        public Action<GameConnectionStateCtx> OnGameConnectionStateChanged;

        public AuthenticationService(Client client)
        {
            this._client = client;
            _logger = _client.Logger;
        }

        public Task Login()
        {
            _autoReconnect = true;
            return GetAuthenticationScene();
        }

        public async Task Logout()
        {
            _autoReconnect = false;
            var gameState = _gameConnectionStateCtx.State;
            if (gameState != GameConnectionState.Disconnected && gameState != GameConnectionState.Disconnecting)
            {
                SetConnectionState(new GameConnectionStateCtx(GameConnectionState.Disconnecting));
                var authScene = await GetAuthenticationScene();

                try
                {
                    await authScene.Disconnect();
                }
                catch(Exception exception)
                {
                    _logger.Log(Diagnostics.LogLevel.Error, "authentication", "An error occurred while disconnecting authentication scene", exception);
                }
            }
        }

        public void SetOperationHandler(string operation, Func<OperationCtx,Task> handler)
        {
            _operationHandlers.Add(operation, handler);
        }

        public async Task<Scene> ConnectToPrivateScene(string sceneId, Action<Scene> sceneBuilder)
        {
            var authScene = await GetAuthenticationScene();
            var sceneToken = await authScene.RpcTask<string, string>("sceneauthorization.gettoken", sceneId);
            return await ConnectToPrivateSceneByToken(sceneToken, sceneBuilder);
        }

        public Task<Scene> ConnectToPrivateSceneByToken(string token, Action<Scene> sceneBuilder)
        {
            return _client.ConnectToPrivateScene(token, scene => { sceneBuilder(scene); });
        }

        public async Task<Scene> GetSceneForService(string serviceType, string serviceName = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            var authScene = await GetAuthenticationScene(cancellationToken);
            var serializer = authScene.DependencyResolver.Resolve<ISerializer>();
            var token = await authScene.RpcTask<string>("Locator.GetSceneConnectionToken", (stream) =>
            {
                serializer.Serialize(serviceType, stream);
                serializer.Serialize(serviceName, stream);
            }, cancellationToken);
            return await _client.ConnectToPrivateScene(token, (scene) => { } , cancellationToken);
        }

        public async Task<Scene> GetAuthenticationScene(CancellationToken ct = default(CancellationToken))
        {
            if (_authTask == null)
            {
                if (!_autoReconnect)
                {
                    throw new InvalidOperationException("Authenticator disconnected. Call login before using the authenticationService.");
                }
                else
                {
                    _authTask = LoginImpl(1000);
                }
            }

            TaskCompletionSource<Scene> tcs = new TaskCompletionSource<Scene>();
            if(ct.CanBeCanceled)
            {
                ct.Register(() => tcs.SetCanceled());
            }

            var scene = await _authTask;
            if (_authTask.IsCanceled)
            {
                tcs.SetCanceled();
            }
            else if (_authTask.IsFaulted)
            {
                tcs.SetException(_authTask.Exception);                      
            }
            DebugLogger.Instance.Debug("Authenticated");
            tcs.SetResult(_authTask.Result);

            return await tcs.Task;           
        }
        
        public async Task<TResult> SendRequestToUser<TData, TResult>(string userId, string operation, CancellationToken cancellationToken, params TData[] datas)
        {
            var scene = await GetAuthenticationScene();
            var serializer = scene.DependencyResolver.Resolve<ISerializer>();
            return await scene.RpcTask<TResult>("sendRequest", stream =>
            {
                serializer.Serialize(userId, stream);
                serializer.Serialize(operation, stream);
                foreach (var data in datas)
                {
                    serializer.Serialize(data, stream);
                }
            });
        }

        public async Task SendRequestToUser<TData>(string userId, string operation, CancellationToken cancellationToken, params TData[] datas)
        {
            var scene = await GetAuthenticationScene();
            var serializer = scene.DependencyResolver.Resolve<ISerializer>();
            await scene.RpcVoid("sendRequest", stream =>
            {
                serializer.Serialize(userId, stream);
                serializer.Serialize(operation, stream);
                foreach (var data in datas)
                {
                    serializer.Serialize(data, stream);
                }
            });
        }

        private async Task<Scene> LoginImpl(int retry)
        {
            if (OnGetAuthParameters == null)
            {
                throw new InvalidOperationException("'getCredentialsCallback' must be set before authentication.");
            }

            try
            {
                Scene authenticationScene = await _client.ConnectToPublicScene(SCENE_ID, (scene) =>
                {
                    scene.SceneConnectionStateObservable.Subscribe((connectionState) =>
                    {
                        switch (connectionState.State)
                        {
                            case ConnectionState.Disconnecting:
                                SetConnectionState(new GameConnectionStateCtx(GameConnectionState.Disconnecting));
                                break;
                            case ConnectionState.Disconnected:
                                SetConnectionState(new GameConnectionStateCtx(GameConnectionState.Disconnected, connectionState.Reason));
                                break;
                            case ConnectionState.Connecting:
                                OnGameConnectionStateChanged?.Invoke(new GameConnectionStateCtx(GameConnectionState.Connecting));
                                break;
                            default:
                                break;
                        }
                    });
                    scene.AddProcedure("sendRequest", context =>
                    {
                        OperationCtx operationCtx = new OperationCtx(context);
                        ISerializer serializer = scene.DependencyResolver.Resolve<ISerializer>();
                        operationCtx.Operation = serializer.Deserialize<string>(context.InputStream);
                        operationCtx.OriginId = serializer.Deserialize<string>(context.InputStream);

                        Func<OperationCtx, Task> handle;
                        if (!_operationHandlers.TryGetValue(operationCtx.Operation, out handle))
                        {
                            throw new KeyNotFoundException("Operation handle not found");
                        }
                        return handle(operationCtx);
                    });
                });

                AuthParameters authParameters = (await OnGetAuthParameters());

                LoginResult result = await authenticationScene.RpcTask<AuthParameters, LoginResult>(LOGIN_ROUTE, authParameters);

                if (!result.Success)
                {
                    _autoReconnect = false;
                    SetConnectionState(new GameConnectionStateCtx(GameConnectionState.Disconnected));
                    throw new InvalidOperationException("Login failed : " + result.ErrorMsg);
                }
                else
                {
                    UserId = result.UserId;
                    UserName = result.UserName;
                    SetConnectionState(new GameConnectionStateCtx(GameConnectionState.Authenticated));
                }

                return authenticationScene;
            }
            catch(Exception exception)
            {
                _logger.Log(LogLevel.Error, "authentication", "An error occured while trying to connect to the server.\n"+exception.Message);
                if(_autoReconnect && _gameConnectionStateCtx.State != GameConnectionState.Disconnected)
                {
                    return await Reconnect(retry + 1);
                }
                else
                {
                    return null;
                }
            }
            
        }

        private async Task<Scene> Reconnect(int retry)
        {
            int delay = Math.Min(retry * 1000, 5000);
            SetConnectionState(new GameConnectionStateCtx(GameConnectionState.Reconnecting));
            await Task.Delay(delay);
            return await LoginImpl(retry);
        }

        private void SetConnectionState(GameConnectionStateCtx stateCtx)
        {
            if(_gameConnectionStateCtx.State != stateCtx.State)
            {
                _logger.Log(Diagnostics.LogLevel.Info, "Authentication", "Connection state changed", stateCtx.State.ToString() + " Reason : " + stateCtx.Reason);

                if(stateCtx.State == GameConnectionState.Disconnected)
                {
                    _authTask = null;
                    if(stateCtx.Reason == "User connected elsewhere" || stateCtx.Reason == "Authentication failed" || stateCtx.Reason == "auth.login.new_connection")
                    {
                        _autoReconnect = false;
                        _client.Disconnect();
                    }
                    if(_autoReconnect)
                    {
                        SetConnectionState(new GameConnectionStateCtx(GameConnectionState.Reconnecting));
                    }
                    else
                    {
                        _gameConnectionStateCtx = stateCtx;
                        OnGameConnectionStateChanged?.Invoke(stateCtx);
                    }
                }
                else if(stateCtx.State == GameConnectionState.Reconnecting && _gameConnectionStateCtx.State != GameConnectionState.Reconnecting)
                {
                    _gameConnectionStateCtx = stateCtx;
                    OnGameConnectionStateChanged?.Invoke(stateCtx);
                    _ = GetAuthenticationScene();
                }
                else
                {
                    _gameConnectionStateCtx = stateCtx;
                    OnGameConnectionStateChanged?.Invoke(stateCtx);
                }
            }
        }

    }
}
