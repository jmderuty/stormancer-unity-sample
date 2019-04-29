using Stormancer.Core;
using UniRx;
using System;
using System.Threading.Tasks;
using Stormancer.Diagnostics;

namespace Stormancer.Plugins
{
    public class PartyService
    {
        private Scene _scene;
        private ILogger _logger;
        private bool _clientReady;
        private bool _playerReady;
        private PartyUserDto[] _members;
        private PartySettings _settings;

        public PartyUserDto[] Members => _members;
        public PartySettings Settings => _settings;

        public PartyService(Scene scene)
        {
            _scene = scene;
            _logger = scene.DependencyResolver.Resolve<ILogger>();
            _clientReady = false;
            _playerReady = false;
        }

        // Events
        public Action OnPartyLeft;
        public Action OnPartyJoined;
        public Action OnPartyKicked;
        public Action<GameFinderStatus> OnPartyGameFinderStateUpdated;
        public Action<GameFinderResponse> OnPartyMatchFound;
        public Action<PartyUserDto[]> OnPartyMembersUpdated;
        public Action<PartyUserData> OnPartyUserDataUpdated;
        public Action<PartySettings> OnPartySettingsUpdated;



        public void Initialize()
        {
            _scene.AddRoute("party.updatesettings", packet =>
            {
                MainThread.Post(async () =>
                {
                    var updatedSettings = packet.ReadObject<PartySettingsDto>();
                    SetNewLocalSettings(updatedSettings);
                    try
                    {
                        await SetStormancerReadyStatus(updatedSettings.GameFinderName);
                    }
                    catch (Exception ex)
                    {
                        await _scene.Disconnect();
                        _logger.Log(Diagnostics.LogLevel.Error, "PartyService", "An error occurred while trying to update the party settings", ex.Message);
                    }
                });
            });

            _scene.AddRoute("party.updateuserData", packet =>
            {
                MainThread.Post(() =>
                {
                    var updatedUserData = packet.ReadObject<PartyUserData>();
                    OnPartyUserDataUpdated?.Invoke(updatedUserData);
                });
            });

            _scene.AddRoute("party.updatepartymembers", packet =>
            {
                MainThread.Post(() =>
                {
                    var members = packet.ReadObject<PartyUserDto[]>();
                    _members = members;
                    OnPartyMembersUpdated?.Invoke(members);
                });
            });

            _scene.AddRoute("party.kicked", packet =>
            {
                MainThread.Post(() =>
                {
                    OnPartyKicked?.Invoke();
                });
            });

            _scene.SceneConnectionStateObservable.Subscribe((state) => 
            {
                if(state.State ==  ConnectionState.Connected)
                {
                    OnPartyJoined?.Invoke();
                }
                else if(state.State == ConnectionState.Disconnected)
                {
                    OnPartyLeft?.Invoke();
                }
            });
        }

        public async Task UpdatePartySettings(PartySettingsDto settings)
        {
            await _scene.RpcVoid<PartySettingsDto>("party.updatepartysettings", settings);
        }

        public void UpdatePlayerStatus(PartyUserStatus status)
        {
            _playerReady = false;
            if(status == PartyUserStatus.Ready)
            {
                _playerReady = true;
            }
            SendPlayerPartyStatus();
        }

        public async Task UpdatePlayerData(string data)
        {
            await _scene.RpcVoid<string>("party.updateplayerdata", data);
        }

        public async Task<bool> PromoteLeader(string playerId)
        {
            return await _scene.RpcTask<string, bool>("party.promoteleader", playerId);
        }

        public async Task<bool> KickPlayer(string userId)
        {
            return await _scene.RpcTask<string, bool>("party.kickplayer", userId);
        }

        private async Task SetStormancerReadyStatus(string gameFinderName)
        {
            _clientReady = false;
            if(gameFinderName != "")
            {
                var gameFinderManager = _scene.DependencyResolver.Resolve<GameFinder>();
                await gameFinderManager.ConnectToGameFinder(gameFinderName);
                _clientReady = true;
            }
            else
            {
                _logger.Log(LogLevel.Error, "PartyService", "Player isn't ready. Game finder cannot be null", new { });
            }
        }

        private async Task SendPlayerPartyStatus()
        {
            //Check if player is ready and if the stormancer is ready.
            PartyUserStatus sendStatus;
            if (_playerReady && _clientReady)
            {
                sendStatus = PartyUserStatus.Ready;  
            }
            else
            {
                sendStatus = PartyUserStatus.NotReady;
            }
            try
            {
                await _scene.RpcVoid<PartyUserStatus>("party.updategamefinderplayerstatus", sendStatus);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "PartyService", "An error occurred when party try to update player status", ex);
            }
        }
           
        private void SetNewLocalSettings(PartySettingsDto settingsDto)
        {
            _settings.GameFinderName = settingsDto.GameFinderName;
            _settings.LeaderId = settingsDto.LeaderId;
            _settings.CustomData = settingsDto.CustomData;
            _settings.PartySize = settingsDto.PartySize;
            _settings.StartOnlyIfPartyFull = settingsDto.StartOnlyIfPartyFull;

            OnPartySettingsUpdated?.Invoke(_settings);
        }
    }
}