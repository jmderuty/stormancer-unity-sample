using System;
using System.Threading.Tasks;
using Stormancer.Core;
using UniRx;
using System.Collections.Generic;
using System.Threading;

namespace Stormancer.Plugins
{

    public class GameFinderService
    {
        private readonly Scene _scene;
        private CancellationTokenSource _gameFinderCancellationSource;

        private GameFinderStatus _currentState = GameFinderStatus.Idle;
        public GameFinderStatus CurrentState
        {
            get => _currentState;
            set
            {
                _currentState = value;
                OnGameFinderStatusUpdated(_currentState);
            }
        }


        // Ancien
        public Action<GameFinderStatus> OnGameFinderStatusUpdated { get; set; }
        public Action<GameFinderResponse> OnGameFound { get; set; }
        public Action<string> OnFindGameRequestFailed { get; set; }

        // Gr : nouvelle version.
        public GameFinderService(Scene scene)
        {
            _scene = scene;
        }

        public void Initialize()
        {
            var serializer = _scene.DependencyResolver.Resolve<ISerializer>();
            _scene.AddRoute("match.update", (packet) =>
            {
                MainThread.Post(() =>
                {
                    int gameState = packet.Stream.ReadByte();
                    CurrentState = (GameFinderStatus)gameState;
                    switch (_currentState)
                    {
                        case GameFinderStatus.Success:
                            {
                                var dto = serializer.Deserialize<GameFinderResponseDTO>(packet.Stream);

                                GameFinderResponse response = new GameFinderResponse();
                                response.ConnectionToken = dto.GameToken;
                                response.OptionalParameters = dto.OptionalParameters;
                                OnGameFound(response);
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
                                OnFindGameRequestFailed(reason);
                                CurrentState = GameFinderStatus.Idle;
                            }
                            break;
                    }
                });
            });

            _scene.AddRoute("match.ready.update", (packet) =>
            {
                MainThread.Post(() =>
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
            if(CurrentState != GameFinderStatus.Idle)
            {
                throw new Exception("Already searching !");
            }

            CurrentState = GameFinderStatus.Searching;
            _gameFinderCancellationSource = new CancellationTokenSource();
            try
            {
                var serializer = _scene.DependencyResolver.Resolve<ISerializer>();
                await _scene.RpcVoid("match.find", (stream) => 
                {
                    serializer.Serialize(provider, stream);
                    serializer.Serialize(data, stream);
                }, PacketPriority.MEDIUM_PRIORITY);
            }
            catch (Exception ex)
            {
                _scene.DependencyResolver.Resolve<ILogger>().Log(Diagnostics.LogLevel.Error, "GameFinderService", "An error occurred in FindGame ", ex.Message);
            	if(CurrentState != GameFinderStatus.Idle)
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
            _scene.SendPacket("match.ready.resolve", stream =>
            {
                stream.WriteByte(acceptMatch ? (byte)1 : (byte)0);
            }, PacketPriority.MEDIUM_PRIORITY, PacketReliability.RELIABLE);
        }

        public void Cancel()
        {
            if (CurrentState != GameFinderStatus.Idle)
            {
                _gameFinderCancellationSource.Cancel();
                _scene.SendPacket("match.cancel", _ => { }, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE_ORDERED);
            }
        }
    }
}