using System;
using System.Threading.Tasks;
using Stormancer.Core;
using UniRx;
using System.Collections.Generic;
using System.Threading;
using Stormancer.Diagnostics;

namespace Stormancer.Plugins
{

    public class GameFinderService
    {
        private readonly Scene _scene;
        private CancellationTokenSource _gameFinderCancellationSource;
        private readonly ILogger _logger;

        private GameFinderStatus _currentState = GameFinderStatus.Idle;
        public GameFinderStatus CurrentState {
            get => _currentState;
            set
            {
                _currentState = value;
                RaiseGameFinderStatusUpdated(_currentState);
            }
        }


        // Ancien
        public Action<GameFinderStatus> OnGameFinderStatusUpdated { get; set; }
        public Action<GameFinderResponse> OnGameFound { get; set; }
        public Action<string> OnFindGameRequestFailed { get; set; }

        private void RaiseGameFound(GameFinderResponse response)
        {
            _scene.DependencyResolver.Resolve<SynchronizationContext>().SafePost(() =>
                {
                    try
                    {
                        OnGameFound?.Invoke(response);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Error, "gamefinder", $"an exception occurred when executing OnGameFound: {ex.Message}");
                    }
                });
        }

        private void RaiseFindGameRequestFailed(string reason)
        {
            _scene.DependencyResolver.Resolve<SynchronizationContext>().SafePost(() =>
            {
                try
                {
                    OnFindGameRequestFailed?.Invoke(reason);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, "gamefinder", $"an exception occurred when executing OnGameFinderStatusUpdated: {ex.Message}");
                }
            });
        }

        private void RaiseGameFinderStatusUpdated(GameFinderStatus status)
        {
            _scene.DependencyResolver.Resolve<SynchronizationContext>().SafePost(() =>
            {
                try
                {
                    OnGameFinderStatusUpdated?.Invoke(status);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, "gamefinder", $"an exception occurred when executing OnGameFinderStatusUpdated: {ex.Message}");
                }
            });
        }

        // Gr : nouvelle version.
        public GameFinderService(Scene scene)
        {
            _scene = scene;
            _logger = _scene.DependencyResolver.Resolve<ILogger>();
        }

        public void Initialize()
        {
            var serializer = _scene.DependencyResolver.Resolve<ISerializer>();
            var synchronizationContext = _scene.DependencyResolver.Resolve<SynchronizationContext>();
            _scene.AddRoute("gamefinder.update", (packet) =>
            {
                int gameState = packet.Stream.ReadByte();
                CurrentState = (GameFinderStatus)gameState;
                switch (CurrentState)
                {
                    case GameFinderStatus.Success:
                    {
                        var dto = serializer.Deserialize<GameFinderResponseDTO>(packet.Stream);

                        GameFinderResponse response = new GameFinderResponse();
                        response.ConnectionToken = dto.GameToken;
                        response.OptionalParameters = dto.OptionalParameters;
                        RaiseGameFound(response);
                        CurrentState = GameFinderStatus.Idle;
                    }
                    break;
                    case GameFinderStatus.Canceled:
                        CurrentState = GameFinderStatus.Idle;
                        break;
                    case GameFinderStatus.Failed:
                    {
                        string reason = "";
                        // There is not always a reason to a failed GameFinder request
                        if (packet.Stream.Position < packet.Stream.Length)
                        {
                            reason = serializer.Deserialize<string>(packet.Stream);
                        }
                        RaiseFindGameRequestFailed(reason);
                        CurrentState = GameFinderStatus.Idle;
                    }
                    break;
                }
            });

            _scene.AddRoute("gamefinder.ready.update", (packet) =>
            {
                synchronizationContext.SafePost(() =>
                {
                    ReadyVerificationRequestDto verificationRequestDto = serializer.Deserialize<ReadyVerificationRequestDto>(packet.Stream);
                    ReadyVerificationRequest readyVerificationRequest = new ReadyVerificationRequest();
                    readyVerificationRequest.MatchId = verificationRequestDto.MatchId;
                    readyVerificationRequest.Timeout = verificationRequestDto.Timeout;
                    readyVerificationRequest.MembersCountReady = 0;
                    readyVerificationRequest.Members = new Dictionary<string, Readiness>();
                    foreach (var member in verificationRequestDto.Members)
                    {
                        Readiness ready = (Readiness)member.Value;
                        readyVerificationRequest.Members.Add(member.Key, ready);
                        if (ready == Readiness.Ready)
                        {
                            readyVerificationRequest.MembersCountReady++;
                        }
                    }
                    readyVerificationRequest.MembersCountTotal = verificationRequestDto.Members.Count;
                });
            });
        }

        private async Task FindGameInternal<T>(string provider, T data)
        {
            if (CurrentState != GameFinderStatus.Idle)
            {
                throw new InvalidOperationException("Already searching !");
            }

            CurrentState = GameFinderStatus.Searching;
            _gameFinderCancellationSource = new CancellationTokenSource();
            try
            {
                var serializer = _scene.DependencyResolver.Resolve<ISerializer>();
                await _scene.RpcVoid("gamefinder.find", (stream) =>
                {
                    serializer.Serialize(provider, stream);
                    serializer.Serialize(data, stream);
                }, PacketPriority.MEDIUM_PRIORITY);
            }
            catch (Exception ex)
            {
                _scene.DependencyResolver.Resolve<ILogger>().Log(Diagnostics.LogLevel.Error, "GameFinderService", "An error occurred in FindGame ", ex.Message);
                if (CurrentState != GameFinderStatus.Idle)
                {
                    CurrentState = GameFinderStatus.Idle;
                }
            }
        }

        /// <summary>
        /// #48
        /// (2018/05/07) : nouvelle version, on passe le pseudo ET une liste de profile du plus favori au moins favori.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task FindGame<T>(string provider, T data)
        {
            return FindGameInternal(provider, data);
        }

        void Resolve(bool acceptMatch)
        {
            _scene.Send("gamefinder.ready.resolve", stream =>
            {
                stream.WriteByte(acceptMatch ? (byte)1 : (byte)0);
            }, PacketPriority.MEDIUM_PRIORITY, PacketReliability.RELIABLE);
        }

        public void Cancel()
        {
            if (CurrentState != GameFinderStatus.Idle)
            {
                _gameFinderCancellationSource.Cancel();
                _scene.Send("gamefinder.cancel", _ => { }, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE_ORDERED);
            }
        }
    }
}