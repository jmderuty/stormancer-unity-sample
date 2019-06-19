
using Stormancer.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UniRx;

namespace Stormancer.Plugins
{
    public class GameSession
    {
        private Client _client;
        private Task<GameSessionContainer> _currentGameSession;
        private bool _useTunnel;
        public Action<IP2PScenePeer> OnPeerConnected { get; set; }
        public Scene GetScene()
        {
            if(_currentGameSession.IsCompleted)
            {
                return _currentGameSession.Result.Scene;
            }
            return null;
        }

        public GameSession(Client client)
        {
            _client = client;
            _currentGameSession = Task.FromResult(new GameSessionContainer());
        }

        public async Task<IEnumerable<SessionPlayer>> GetConnectedPlayer()
        {
            var gameSessionContainer = await _currentGameSession;
            var service = gameSessionContainer.Service;
            return service.ConnectedPlayers;
        }

        /// <summary>
        /// Connect to the game session
        /// </summary>
        /// <param name="token"> Connection token to the game session scene</param>
        /// <param name="mapName"> Name of the map of the game session</param>
        /// <param name="cancellationToken"> Connection cancellation token </param>
        /// <returns> The parameters to connect to the game session</returns>
        public async Task ConnectToGameSession(string token, bool useTunnel, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!_currentGameSession.IsCompleted)
            {
                throw new InvalidOperationException("Game session connection is in pending. Cannot start another connection to GameSession");
            }
            if (token.Length == 0)
            {
                throw new InvalidOperationException("Game session can't be connected without token");
            }
            // Disconnect from gameSession if the current ct equal the pending CT.
           
            await DisconnectFromGameSession();
            _currentGameSession = ConnectToGameSessionImpl(token, useTunnel, cancellationToken);
            _useTunnel = useTunnel;
        }

        public async Task<GameSessionConnectionParameters> EstablishDirectConnection(CancellationToken cancellationToken = default(CancellationToken))
        {
            var container = await _currentGameSession;
            var p2pToken = await P2PTokenRequest(container, cancellationToken);
            var service = container.Service;
            var logger = container.Scene.DependencyResolver.Resolve<ILogger>();
            try
            {
                await service.InitializeTunnel(p2pToken, _useTunnel, cancellationToken);
            }
            catch (Exception ex) when (!(ex is OperationCanceledException))
            {
                logger.Log(Diagnostics.LogLevel.Error, "GameSession", "An error occurred during EstablishDirectConnection : " + ex.Message, ex);
                throw;
            }
            GameSessionConnectionParameters connectionParameters = new GameSessionConnectionParameters();

            try
            {
                connectionParameters = await container.GameSessionReadyTask;
            }
            catch (Exception)
            {
                try
                {
                    await DisconnectFromGameSession();
                }
                catch (Exception ex)
                {
                    logger.Log(Diagnostics.LogLevel.Warn, "GameSession", "Cannot disconnect from game session after connection timeout or cancel.", ex.Message);
                    throw ex;
                }
            }
            return connectionParameters;
        }

        /// <summary>
        /// Set the player ready to start the game session. The game session starts only when all players are ready
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken></param>
        /// <returns></returns>
        public async Task SetPlayerReady(string data, CancellationToken cancellationToken = default(CancellationToken))
        {
            var container = await GetCurrentGameSession(cancellationToken);
            if(container != null)
            {
                container.Service.Ready(data);
            }
            else
            {
                throw new InvalidOperationException("Not connected to any game session");
            }
        }

        /// <summary>
        /// Send the game post result to the server
        /// </summary>
        /// <param name="gameSessionResult"> Player result of the current game session</param>
        /// <param name="cancellationToken"></param>
        /// <returns> The result of all players in the game session</returns>
        public async Task<GameSessionResult> PostResult(EndGameDto gameSessionResult, CancellationToken cancellationToken = default(CancellationToken))
        {
            var container = await GetCurrentGameSession(cancellationToken);
            if (container != null)
            {
                var result = await container.Service.SendGameResult<EndGameDto, GameSessionResult>(gameSessionResult, cancellationToken);
                return result;
            }
            else
            {
                throw new InvalidOperationException("Not connected to any game session");
            }
        }

        /// <summary>
        /// Get the userId of the token's owner
        /// </summary>
        /// <param name="token"></param>
        /// <returns> The user id of the token's owner</returns>
        public async Task<string> GetUserFromBearerToken(string token)
        {
            var container = await GetCurrentGameSession();
            if (container != null)
            {
                return await container.Service.GetUserFromBearerToken(token);
            }
            else
            {
                throw new InvalidOperationException("Not connected to any game session");
            }
        }

        /// <summary>
        /// Disconnect from the current GameSession
        /// </summary>
        /// <returns></returns>
        public async Task DisconnectFromGameSession()
        {
            try
            {
                var container = await GetCurrentGameSession();
                if (container != null && container.Service != null)
                {
                    await container.Service.Disconnect();
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Internal implementation of the connection to the game session
        /// </summary>
        /// <param name="token"> Token of the game session scene</param>
        /// <param name="cancellationToken"></param>
        /// <returns> The container of the game session</returns>
        private async Task<GameSessionContainer> ConnectToGameSessionImpl(string token, bool useTunnel, CancellationToken cancellationToken)
        {
            var gameSessionScene  = await _client.ConnectToPrivateScene(token, (scene) => { }, cancellationToken);
            var container = new GameSessionContainer();
            container.Scene = gameSessionScene;
            OnGameSessionConnectionChanged?.Invoke(new ConnectionStateCtx(ConnectionState.Connected));
            TaskCompletionSource<GameSessionConnectionParameters> sessionReadyTcs = new TaskCompletionSource<GameSessionConnectionParameters>();
            container.GameSessionReadyTask = sessionReadyTcs.Task;

            gameSessionScene.SceneConnectionStateObservable.Subscribe((state) =>
            {
                if(state.State == ConnectionState.Disconnected)
                {
                    MainThread.Post(() =>
                    {
                        OnGameSessionConnectionChanged?.Invoke(state);
                    });
                    _currentGameSession = Task.FromResult<GameSessionContainer>(null);
                }
            });

            gameSessionScene.OnPeerConnected += peer =>
            {
                OnPeerConnected?.Invoke(peer);
            };

            container.Service.OnRoleReceived += (role) =>
            {
                GameSessionConnectionParameters parameters = new GameSessionConnectionParameters();
                if(parameters.IsHost || role == "CLIENT" && !useTunnel)
                {
                    parameters.IsHost = (role == "HOST");
                    OnRoleReceived?.Invoke(parameters);
                    sessionReadyTcs.SetResult(parameters);
                }
            };
            if(useTunnel)
            {
                container.Service.OnTunnelOpened += (p2pTunnel) =>
                {
                    GameSessionConnectionParameters parameters = new GameSessionConnectionParameters();
                    parameters.IsHost = false;
                    parameters.Endpoint = p2pTunnel.Ip + ":" + p2pTunnel.Port;
                    OnTunnelOpened?.Invoke(parameters);
                    sessionReadyTcs.SetResult(parameters);
                };
            }

            container.Service.OnAllPlayerReady += () =>
            {
                OnAllPlayerReady?.Invoke();
            };

            container.Service.OnShutdownReceived += () =>
            {
                OnShutdownReceived?.Invoke();
            };

            return container;
        }

        /// <summary>
        /// Request a P2P token
        /// </summary>
        /// <param name="container"> Game session container</param>
        /// <param name="cancellationToken"></param>
        /// <returns> The p2p token</returns>
        private async Task<string> P2PTokenRequest(GameSessionContainer container, CancellationToken cancellationToken)
        {
            if(container != null)
            {
                return await container.Service.P2PTokenRequest(cancellationToken);
            }
            else
            {
                throw new InvalidOperationException("game session doesn't exist");
            }
        }

        /// <summary>
        /// Get the current game session container
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>The current game session container</returns>
        private async Task<GameSessionContainer> GetCurrentGameSession(CancellationToken cancellationToken = default(CancellationToken))
        {
            var container = await _currentGameSession;
            cancellationToken.ThrowIfCancellationRequested();
            return container;
        }

        #region events

        public Action OnAllPlayerReady { get; set; }

        public Action<GameSessionConnectionParameters> OnRoleReceived { get; set; }

        public Action<GameSessionConnectionParameters> OnTunnelOpened { get; set; }

        public Action<ConnectionStateCtx> OnGameSessionConnectionChanged { get; set; }

        public Action OnShutdownReceived { get; set; }
        public Action<Scene> OnConnectingToScene { get; set; }
        public Action<Scene> OnDisconnectingFromScene { get; set; }
        #endregion
    }
}
