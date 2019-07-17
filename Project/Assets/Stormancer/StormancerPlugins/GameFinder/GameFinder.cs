using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Stormancer.Core;
using Stormancer.Diagnostics;
using UniRx;

namespace Stormancer.Plugins
{
    public class GameFinder
    {
        private ConcurrentDictionary<string, Task<GameFinderContainer>> _gameFinders = new ConcurrentDictionary<string, Task<GameFinderContainer>>();
        private ConcurrentDictionary<string, CancellationTokenSource> _pendingFindGameRequest = new ConcurrentDictionary<string, CancellationTokenSource>();
        private AuthenticationService _auth;
        private ILogger _logger;

        public Action<GameFinderStatusChangedEvent> OnGameFinderStateChanged { get; set; }
        public Action<GameFoundEvent> OnGameFound { get; set; }
        public Action<FindGameFailedEvent> OnFindGameFailed { get; set; }


        public GameFinder(AuthenticationService authenticationService, ILogger logger)
        {
            _auth = authenticationService;
            _logger = logger;
        }
        /// <summary>
		/// Cancel an ongoing <c>findGame</c> request.
		/// </summary>
		/// <remarks>
		/// You should call this method only after you have received the initial <c>GameFinderStatusChangedEvent</c> with <c>GameFinderStatus::Searching</c>,
		/// or else you might run into a race condition and the cancel request might not register.
		/// </remarks>
		/// <param name="gameFinder">Name of the GameFinder for which you want to cancel the search.</param>
        public void Cancel(string gameFinder)
        {
            CancellationTokenSource source;
            if(_pendingFindGameRequest.TryGetValue(gameFinder, out source))
            {
                _logger.Log(LogLevel.Trace, "GameFinder", $"Cancelling pending find game request for gamefinder {gameFinder}");
                source.Cancel();
            }
            if(source == null)
            {
                _logger.Log(LogLevel.Trace, "GameFinder", $"No pending find game request for gamefinder {gameFinder}");
            }
        }

        /// <summary>
		/// Connect to the scene that contains the given GameFinder.
		/// </summary>
		/// <remarks>This will use the server application's ServiceLocator configuration to determine which scene to connect to for the given <c>gameFinderName</c>.</remarks>
		/// <param name="gameFinderName">Name of the GameFinder to connect to.</param>
		/// <returns>A <c>pplx::task</c> that completes when the connection to the scene that contains <c>gameFinderName</c> has completed.</returns>
        public async Task ConnectToGameFinder(string gameFinderName)
        {
            await GetGameFinderContainer(gameFinderName);
        }

        /// <summary>
		/// Disconnect from the scene that contains the given GameFinder.
		/// </summary>
		/// <param name="gameFinderName">Name of the GameFinder which scene you want to disconnect from.</param>
		/// <returns>A <c>pplx::task</c> that completes when the scene disconnection has completed.</returns>
        public async Task DisconnectFromGameFinder(string gameFinderName)
        {
            Task<GameFinderContainer> containerTask;
            if(_gameFinders.TryRemove(gameFinderName, out containerTask))
            {
                var container = await containerTask;
                await container.Scene.Disconnect();
            }
        }

        /// <summary>
		/// Start a GameFinder query.
		/// Only if you do not use the Party system.
		/// </summary>
		/// <remarks>
		/// This method will attempt to connect to the server and the scene for the given <c>gameFinder</c> if the client is not yet connected to them.
		/// After the query has started, the server will notify you when a status update occurs.
		/// You should listen to these updates by providing callbacks to <c>subsribeGameFinderStateChanged()</c> and <c>subsribeGameFound()</c>.
		/// If you want to cancel the request, you should call <c>cancel()</c>, with the same <c>gameFinder</c> as the one passed to <c>findGame()</c>.
		/// We use this technique here instead of the more common <c>cancellation_token</c>-based one in order to support party scenarios,
		/// where a member of a party can cancel the party-wide GameFinder query.
		/// For parties:
		/// Do not use this method if you are in a party, as the GameFinder query will be initiated automatically by the server when all party members are ready.
		/// </remarks>
		/// <param name="gameFinder">Name of the server-side GameFinder to connect to.
		/// This will typically be the name of a scene, configured in the serviceLocator of the server application.</param>
		/// <param name="provider">Name of the provider to use for the given <c>gameFinder</c>.</param>
		/// <param name="json">Custom JSON data to send along the FindGame request.</param>
		/// <returns>A <c>task</c> that completes when the request is done.
		/// This task will complete when either one of the following happens:
		/// * A game is found
		/// * An error occurs on the server-side GameFinder
		/// * The request is canceled with a call to <c>cancel()</c>.</returns>
        public async Task FindGame<TData>(string gameFinder, string provider, TData data)
        {
            if (!_pendingFindGameRequest.TryAdd(gameFinder, new CancellationTokenSource()))
            {
                throw new InvalidOperationException($"A findGame request is already running for GameFinder '{gameFinder}'");
            }
            var cancellationToken = _pendingFindGameRequest[gameFinder].Token;
            var container = await GetGameFinderContainer(gameFinder);
            if(cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            cancellationToken.Register(() => container.Service.Cancel());
            await container.Service.FindGame(provider, data);
            _pendingFindGameRequest.TryRemove(gameFinder, out _);
        }

        /// <summary>
        /// Retrieve the current status of ongoing <c>findGame</c> requests for each GameFinder.
        /// </summary>
        /// <returns>A map with the GameFinder name as key, and the <c>findGame</c> request status as value.</returns>
        public Dictionary<string, GameFinderStatusChangedEvent> GetPendingFindGameStatus()
        {
            Dictionary<string, GameFinderStatusChangedEvent> result = new Dictionary<string, GameFinderStatusChangedEvent>();
            foreach(var gamefinder in _gameFinders)
            {
                GameFinderStatusChangedEvent status = new GameFinderStatusChangedEvent();
                status.GameFinder = gamefinder.Key;
                var task = gamefinder.Value;
                if(task.IsCompleted)
                {
                    var container = task.Result;
                    status.Status = container.Service.CurrentState;
                }
                else
                {
                    status.Status = GameFinderStatus.Loading;
                }
                result.Add(gamefinder.Key, status);
            }
            return result;
        }

        private async Task<GameFinderContainer> ConnectToGameFinderImpl(string gameFinderName)
        {
            if(_auth == null)
            {
                throw new InvalidOperationException("Authentication service destroyed");
            }
            GameFinderContainer container = new GameFinderContainer();
            try
            {
                var scene = await _auth.GetSceneForService("stormancer.plugins.gamefinder", gameFinderName);
                container.Scene = scene;
                container.Scene.SceneConnectionStateObservable.Subscribe((state) =>
                {
                    if(state.State == ConnectionState.Disconnecting)
                    {
                        _gameFinders.TryRemove(gameFinderName, out _);
                    }
                });
                container.Service.OnGameFound += (response) =>
                {
                    GameFoundEvent foundEvent = new GameFoundEvent();
                    foundEvent.GameFinder = gameFinderName;
                    foundEvent.Data = response;
                    OnGameFound?.Invoke(foundEvent);
                };
                container.Service.OnGameFinderStatusUpdated += (status) =>
                {
                    GameFinderStatusChangedEvent changedEvent = new GameFinderStatusChangedEvent();
                    changedEvent.GameFinder = gameFinderName;
                    changedEvent.Status = status;
                    OnGameFinderStateChanged?.Invoke(changedEvent);
                };
                container.Service.OnFindGameRequestFailed += (reason) =>
                {
                    FindGameFailedEvent failedEvent = new FindGameFailedEvent();
                    failedEvent.GameFinder = gameFinderName;
                    failedEvent.Reason = reason;
                    OnFindGameFailed?.Invoke(failedEvent);
                };
            }
            catch (System.Exception ex)
            {
                throw new InvalidOperationException($"Failed to connect to game finder. sceneName= {gameFinderName} reason= {ex.Message}");
            }
            return container;

        }

        private Task<GameFinderContainer> GetGameFinderContainer(string gameFinderName)
        {
            Task<GameFinderContainer> container;
            if(_gameFinders.TryGetValue(gameFinderName, out container))
            {
                return container;
            }
            else
            {
                container = ConnectToGameFinderImpl(gameFinderName);
                _gameFinders.TryAdd(gameFinderName, container);
                return container;
            }
        }
    }
}
