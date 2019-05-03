using Stormancer.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using UniRx;

namespace Stormancer.Plugins
{
    public class Party : ClientAPI<Party>
    {
        private PartyInvitations _invitations = new PartyInvitations();
        private ILogger _logger;
        private GameFinder _gameFinder;
        private ISerializer _serializer;
        private string _uniqueOnlinePartyName;
        private Task<PartyContainer> _party;

        public PartyInvitations Invitations => _invitations;

        public bool IsInParty => _party != null;

        public Action<PartySettings> OnPartySettingsUpdated;
        public Action<PartyUserDto[]> OnPartyMembersUpdated;
        public Action<PartyUserData> OnUserDataUpdated;
        public Action<PartyInvitation[]> OnInvitationsUpdate;
        public Action OnPartyJoined;
        public Action OnPartyLeft;
        public Action OnPartyKicked;
        private IDisposable _partySceneConnectionStateSubscription;

        public Party(AuthenticationService authenticationService, ILogger logger, ISerializer serializer, GameFinder gameFinder) : base(authenticationService)
        {
            _logger = logger;
            _gameFinder = gameFinder;
            _serializer = serializer;
        }

        public void Initialize()
        {
            _auth.SetOperationHandler("party.invite", context =>
            {
                TaskCompletionSource<Unit> tcs = new TaskCompletionSource<Unit>(context.RequestContext.CancellationToken);
                MainThread.Post(() =>
                {

                    var senderId = context.OriginId;
                    var sceneId = _serializer.Deserialize<string>(context.RequestContext.InputStream);
                    var invitation = _invitations.ReceivePartyInvitation(senderId, sceneId);
                    invitation.OnAnswer += answer =>
                    {
                        context.RequestContext.SendValue(steam => { }, Core.PacketPriority.MEDIUM_PRIORITY);
                        tcs.SetResult(new Unit());
                    };
                    context.RequestContext.CancellationToken.Register(() =>
                    {
                        tcs.TrySetCanceled();
                        _invitations.RemovePartyInvitation(senderId);
                    });
                });
                return tcs.Task;
            });

            _invitations.OnInvitationsUpdate += invitations =>
            {
                OnInvitationsUpdate?.Invoke(invitations);
            };
        }

        public async Task<PartyContainer> CreateParty(PartyRequestDto partySettings)
        {
            if(_party != null)
            {
                throw new InvalidOperationException("Already in a party");
            }
            var managementService = await GetPartyManagementService();
            var sceneToken = await managementService.CreateParty(partySettings);
            return await JoinPartySceneByConnectionToken(sceneToken);
        }

        public async Task<PartyContainer> JoinPartySceneByPlatformSessionId(string uniqueOnlinePartyName)
        {
            if (_party != null)
            {
                throw new InvalidOperationException("Already in a party");
            }
            PartyContainer container;
            try
            {
                await LeaveParty();
                container = await GetPartySceneByOnlinePartyName(uniqueOnlinePartyName);
                OnPartyJoined?.Invoke();
            }
            catch (System.Exception ex)
            {
                _logger.Log(Diagnostics.LogLevel.Error, "PartyManagement", "Failed to get the party scene", ex);
                _party = null;
                throw ex;
            }
            _party = Task.FromResult(container);
            return container;
        }

        public async Task<PartyContainer> JoinPartySceneByConnectionToken(string connectionToken)
        {
            if (_party != null)
            {
                throw new InvalidOperationException("Already in a party");
            }
            PartyContainer container;
            try
            {
                container = await GetPartySceneByToken(connectionToken);
                OnPartyJoined?.Invoke();
            }
            catch (System.Exception ex)
            {
                _logger.Log(Diagnostics.LogLevel.Error, "PartyManagement", "Failed to get the party scene", ex);
                _party = null;
                throw ex;
            }
            _party = Task.FromResult(container);
            return container;
        }

        public async Task LeaveParty()
        {
            if(_party == null)
            {
                _logger.Log(Diagnostics.LogLevel.Warn, "PartyManagement", "Client not connected on party");
            }
            else
            {
                var container = await _party;
                await container.PartyScene.Disconnect();
                _party = null;
            }
        }

        public async Task<PartyContainer> GetParty()
        {
            if(_party != null)
            {
                return await _party;
            }
            else
            {
                throw new InvalidOperationException("Not in a party");
            }
        }

        public async Task UpdatePlayerStatus(PartyUserStatus userStatus)
        {
            var container = await GetParty();
            var service = container.PartyScene.DependencyResolver.Resolve<PartyService>();
            service.UpdatePlayerStatus(userStatus);
        }

        public async Task UpdatePlayerData(string data)
        {
            var container = await GetParty();
            var service = container.PartyScene.DependencyResolver.Resolve<PartyService>();
            await service.UpdatePlayerData(data);
        }

        public async Task<bool> PromoteLeader(string userId)
        {
            var container = await GetParty();
            var service = container.PartyScene.DependencyResolver.Resolve<PartyService>();
            return await service.PromoteLeader(userId);
        }

        public async Task<bool> KickPlayer(string userId)
        {
            var container = await GetParty();
            var service = container.PartyScene.DependencyResolver.Resolve<PartyService>();
            return await service.KickPlayer(userId);
        }

        public async Task SendInvitation(string userId)
        {
            var partyContainer = await GetParty();
            var senderId = _auth.UserId;
            var partyId = partyContainer.Id;
            CancellationTokenSource cts = new CancellationTokenSource();

            _invitations.SendPartyRequest(userId, cts);
            await _auth.SendRequestToUser(userId, "party.invite", cts.Token, senderId, partyId);
            _invitations.CancelPartyRequest(userId);
        }

        private async Task<PartyContainer> GetPartySceneByOnlinePartyName(string uniqueOnlinePartyName)
        {
            var scene = await _auth.GetSceneForService("stormancer.plugins.party", uniqueOnlinePartyName);
            return InitPartyFromScene(scene);
        }

        private async Task<PartyContainer> GetPartySceneByToken(string connectionToken)
        {
            var scene = await _auth.ConnectToPrivateSceneByToken(connectionToken, (s)=> { });
            return InitPartyFromScene(scene);
        }

        private PartyContainer InitPartyFromScene(Scene scene)
        {
            var partyService = scene.DependencyResolver.Resolve<PartyService>();
            _partySceneConnectionStateSubscription = scene.SceneConnectionStateObservable.Subscribe(state =>
            {
                if(state.State == ConnectionState.Disconnected)
                {
                    OnPartyLeft?.Invoke();
                    _partySceneConnectionStateSubscription.Dispose();
                }
            });
            var container = new PartyContainer(scene);
            partyService.OnPartyJoined += () =>
            {
                OnPartyJoined?.Invoke();
            };
            partyService.OnPartyKicked += () =>
            {
                OnPartyKicked?.Invoke();
            };
            partyService.OnPartyLeft += async () =>
            {
                if(_party != null)
                {
                    var partyContainer = await _party;
                    var gameFinderName = partyContainer.Settings.GameFinderName;
                    _party = null;
                    await _gameFinder.DisconnectFromGameFinder(gameFinderName);
                    OnPartyLeft?.Invoke();
                }
            };
            partyService.OnPartyMembersUpdated += members =>
            {
                OnPartyMembersUpdated?.Invoke(members);
            };
            partyService.OnPartyUserDataUpdated += data =>
            {
                OnUserDataUpdated?.Invoke(data);
            };
            partyService.OnPartySettingsUpdated += settings =>
            {
                OnPartySettingsUpdated?.Invoke(settings);
            };
            return container;
        }

        private async Task<PartyManagementService> GetPartyManagementService()
        {
            return await GetService<PartyManagementService>("stormancer.plugins.partyManagement");
        }
    }
}