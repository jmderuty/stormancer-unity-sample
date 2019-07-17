using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using Stormancer.Diagnostics;

namespace Stormancer.Plugins
{
    public class GameSessionService
    {
        private class SessionPlayerUpdateArg
        {
            public SessionPlayer Player { get; set; }
            public string Data { get; set; }

            public SessionPlayerUpdateArg(SessionPlayer player, string data)
            {
                Player = player;
                Data = data;
            }
        }

        private readonly Scene _scene;
        private readonly ILogger _logger;
        private bool _receivedP2PToken = false;
        private string GAMESESSION_P2P_SERVER_ID = "GameSession";
        private P2PTunnel _tunnel;
        private readonly CancellationTokenSource _disconnectionCts = new CancellationTokenSource();
        private readonly ConcurrentDictionary<string, SessionPlayer> _users = new ConcurrentDictionary<string, SessionPlayer>();
        private readonly TaskCompletionSource<bool> _waitServerTcs = new TaskCompletionSource<bool>();

        public Action<string> OnConnectionFailure { get; private set; }
        public Action<IP2PScenePeer> OnConnectionOpened { get; private set; }
        public Action OnShutdownReceived { get; set; }
        public Action OnAllPlayerReady { get; set; }
        public Action<string> OnRoleReceived { get; set; }
        public Action<P2PTunnel> OnTunnelOpened { get; set; }

        public bool ShouldEstablishTunnel { get; private set; } = true;

        public IEnumerable<SessionPlayer> ConnectedPlayers
        {
            get
            {
                return _users.Values;
            }
        }

        private void RaiseTunnelOpened(P2PTunnel tunnel)
        {
            var synchronizationContext = _scene.DependencyResolver.Resolve<SynchronizationContext>();
            synchronizationContext.SafePost(() =>
            {
                try
                {
                    OnTunnelOpened?.Invoke(tunnel);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, "gamefinder", $"an exception occurred when executing OnTunnelOpened: {ex.Message}");
                }
            });
        }

        private void RaiseRoleReceived(string role)
        {
            var synchronizationContext = _scene.DependencyResolver.Resolve<SynchronizationContext>();
            synchronizationContext.SafePost(() =>
            {
                try
                {
                    OnRoleReceived?.Invoke(role);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, "gamefinder", $"an exception occurred when executing OnRoleReceived: {ex.Message}");
                }
            });
        }

        private void RaiseAllPlayerReady()
        {
            var synchronizationContext = _scene.DependencyResolver.Resolve<SynchronizationContext>();
            synchronizationContext.SafePost(() =>
            {
                try
                {
                    OnAllPlayerReady?.Invoke();
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, "gamefinder", $"an exception occurred when executing OnAllPlayerReady: {ex.Message}");
                }
            });
        }

        private void RaiseShutdownReceived()
        {
            var synchronizationContext = _scene.DependencyResolver.Resolve<SynchronizationContext>();
            synchronizationContext.SafePost(() =>
            {
                try
                {
                    OnShutdownReceived?.Invoke();
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, "gamefinder", $"an exception occurred when executing OnShutdownReceived: {ex.Message}");
                }
            });
        }

        private void RaiseConnectionOpened(IP2PScenePeer p2pScenePeer)
        {
            var synchronizationContext = _scene.DependencyResolver.Resolve<SynchronizationContext>();
            synchronizationContext.SafePost(() =>
            {
                try
                {
                    OnConnectionOpened?.Invoke(p2pScenePeer);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, "gamefinder", $"an exception occurred when executing OnConnectionOpened: {ex.Message}");
                }
            });
        }

        private void RaiseConnectionFailure(string reason)
        {
            var synchronizationContext = _scene.DependencyResolver.Resolve<SynchronizationContext>();
            synchronizationContext.SafePost(() =>
            {
                try
                {
                    OnConnectionFailure?.Invoke(reason);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, "gamefinder", $"an exception occurred when executing OnConnectionFailure: {ex.Message}");
                }
            });
        }
        public GameSessionService(Scene scene)
        {
            this._scene = scene;
            _logger = scene.DependencyResolver.Resolve<ILogger>();
        }

        public void Initialize()
        {
            var synchronizationContext = _scene.DependencyResolver.Resolve<SynchronizationContext>();
            _scene.AddRoute("gameSession.shutdown", (packet) =>
            {
                RaiseShutdownReceived();
            });

            _scene.AddRoute<PlayerUpdate>("player.update", update =>
            {
                synchronizationContext.SafePost(() =>
                {
                    var player = new SessionPlayer(update.UserId, (PlayerStatus)update.Status);
                    _users.AddOrUpdate(update.UserId, player, (_, __) => player);
                });

            });

            _scene.AddRoute("players.allReady", packet =>
            {
                RaiseAllPlayerReady();
            });
        }

        public async Task InitializeTunnel(string p2pToken, bool useTunnel, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken = LinkTokenToDisconnection(cancellationToken);
            if (_scene == null)
            {
                _logger.Log(LogLevel.Error, "GameSessionService.InitializeTunnel", "scene deleted");
                throw new InvalidOperationException("scene deleted");
            }

            _logger.Log(LogLevel.Trace, "GameSessionService.InitializeTunnel", "received p2p token");
            if (_receivedP2PToken)
            {
                _logger.Log(LogLevel.Error, "GameSessionService.InitializeTunnel", "Already received P2P token");
                throw new InvalidOperationException("Already received P2P token");
            }
            _receivedP2PToken = true;
            if(p2pToken == null)
            {
                _logger.Log(LogLevel.Trace, "GameSessionService.InitializeTunnel", "received empty p2p token : I'm the host."); 
                RaiseRoleReceived("HOST");
                if(useTunnel)
                {
                    _tunnel = _scene.RegisterP2PServer(GAMESESSION_P2P_SERVER_ID);
                    _logger.Log(LogLevel.Trace, "GameSessionService.InitializeTunnel", "Tunnel established on host");
                }
            }
            else
            {
                _logger.Log(LogLevel.Trace, "GameSessionService.InitializeTunnel", "received empty p2p token : I'm a client.");
                try
                {
                    var p2pPeer = await _scene.OpenP2PConnection(p2pToken, cancellationToken);
                    RaiseRoleReceived("CLIENT");
                    RaiseConnectionOpened(p2pPeer);
                    if (useTunnel)
                    {
                        _tunnel = await p2pPeer.OpenP2PTunnel(GAMESESSION_P2P_SERVER_ID, cancellationToken);
                        _logger.Log(LogLevel.Trace, "GameSessionService.InitializeTunnel", "Tunnel established on client");

                        RaiseTunnelOpened(_tunnel);
                    }
                }
                catch (Exception ex) when (!(ex is OperationCanceledException))
                {
                    RaiseConnectionFailure(ex.Message);
                    _logger.Log(LogLevel.Error, "GameSessionService", "An error occurred in InitializeTunnel : "+ex.Message, ex);
                    throw;
                }
            }
        }

        private CancellationToken LinkTokenToDisconnection(CancellationToken cancellationToken)
        {
            if(cancellationToken.CanBeCanceled)
            {
                return CancellationTokenHelpers.CreateLinkedSource(cancellationToken, _disconnectionCts.Token).Token;
            }
            else
            {
                return _disconnectionCts.Token;
            }
        }

        public async Task<string> GetUserFromBearerToken(string token)
        {
            return await _scene.RpcTask<string, string>("GameSession.GetUserFromBearerToken", token);
        }

        public async Task<string> P2PTokenRequest(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken = LinkTokenToDisconnection(cancellationToken);
            return await _scene.RpcTask<int, string>("GameSession.GetP2PToken", 1, cancellationToken);
        }

        public async Task Reset(CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken = LinkTokenToDisconnection(cancellationToken);
            await _scene.RpcTask("gamesession.reset", stream => { }, cancellationToken);
        }

        public async Task Disconnect()
        {
            await _scene.Disconnect();
        }

        public void OnDisconnecting()
        {
            _tunnel?.Release();
            _users.Clear();
            _disconnectionCts.Cancel();
        }

        public void Ready(string data)
        {
            _scene.Send("player.ready", data);
        }

        public Task<TOut> SendGameResult<Tin, TOut>(Tin result, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _scene.RpcTask<Tin, TOut>("gamesession.postresults", result, cancellationToken);
        }

    }
}