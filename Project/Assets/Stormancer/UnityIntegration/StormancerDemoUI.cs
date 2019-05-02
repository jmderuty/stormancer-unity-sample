using Stormancer;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using Stormancer.Plugins;
using UnityEngine.SceneManagement;
using System.Threading;
using System;
using System.Collections.Generic;

// Those PluginProertyHolder are used to hide/display all information for each plugin in the inspector
[System.Serializable]
public class AuthenticationPropertyHolder
{
    public UnityEvent OnConnected;
    public UnityEvent OnDisconnected;
    // OnDisconnected event is called before OnReconnecting (only graphically)
    public UnityEvent OnReconnecting;
}

[System.Serializable]
public class GameFinderPropertyHolder
{
    public Text GameFinderStatusText;
    public UnityEvent OnMatchFound;
}

[System.Serializable]
public class GameSessionPropertyHolder
{
    public UnityEvent OnAllPlayerReady;
    public UnityEvent OnSessionLeaved;
    public UnityEvent OnGameResultReceived;
    public CancellationTokenSource PostResultCancellationSource;
}

[System.Serializable]
public class PartyPropertyHolder
{
    public Transform PartyUserPanel;
    public Transform PartyInvitationPanel;
    public GameObject LeaderUI;
    public InputField UserIdTextField;
    public UnityEvent OnGameFound;
    public UnityEvent OnPartyLeaved;
    public UnityEvent OnPartyJoined;
}
[System.Serializable]
public class PartyInvitationAnswer
{
    public Text SenderIdText;
    public bool Accept;
}

    public class StormancerDemoUI : MonoBehaviour
{
    public InputField UserIdField;

    public AuthenticationPropertyHolder Authentication; 

    public GameFinderPropertyHolder GameFinder;

    public GameSessionPropertyHolder GameSession;

    public PartyPropertyHolder Party;

    private GameSessionResult _lastGameResult;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        var auth = ClientProvider.GetService<AuthenticationService>();
        UserIdField.text = auth.UserId;
    }

    #region Authentication

    public void Login()
    {
        _ = LoginAsync();
    }

    private async Task LoginAsync()
    {
        try
        {
            var auth = ClientProvider.GetService<AuthenticationService>();
            auth.OnGameConnectionStateChanged += CheckConnectionState;
            await auth.Login();
            // this is useful to initialize Party (especially to set the operation handler to party.invite, that allows us to receive party invitations)
            RegisterPartyCallback();
            RegisterGameFinderCallback();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred on Login "+ex.Message);
        }
    }

    private void OnDestroy()
    {
        ClientProvider.CloseClient();
    }

    private void CheckConnectionState(GameConnectionStateCtx gameConnectionStateCtx)
    {
        if(gameConnectionStateCtx.State == GameConnectionState.Authenticated)
        {
            Authentication.OnConnected.Invoke();
        }
        else if(gameConnectionStateCtx.State == GameConnectionState.Disconnected)
        {
            Authentication.OnDisconnected.Invoke();
        }
        else if (gameConnectionStateCtx.State == GameConnectionState.Reconnecting)
        {
            Authentication.OnDisconnected.Invoke();
            Authentication.OnReconnecting.Invoke();
        }
    }
    #endregion

    #region GameFinder

    public void StartMatchmaking()
    {
        _ = StartMatchmakingAsync();
    }
    
    private async Task StartMatchmakingAsync()
    {
        try
        {
            var gameFinder = ClientProvider.GetService<GameFinder>();
            await gameFinder.FindGame("matchmakerdefault", "json", "{}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred during StartMatchmaking " + ex.Message);
        }
    }

    public void CancelMatchmaking()
    {
        try
        {
            var gameFinder = ClientProvider.GetService<GameFinder>();
            gameFinder.Cancel("matchmakerdefault");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred during CancelMatchMaking " + ex.Message);
        }
    }
    
    public void RegisterGameFinderCallback()
    {
        var gameFinder = ClientProvider.GetService<GameFinder>();
        gameFinder.OnGameFinderStateChanged += statusChangedEvent =>
        {
            GameFinder.GameFinderStatusText.text = statusChangedEvent.Status.ToString();
        };
        gameFinder.OnFindGameFailed += failedEvent =>
        {
            Debug.Log("Game finder failed");
        };
        gameFinder.OnGameFound += gameFoundEvent =>
        {
            GameFinder.OnMatchFound.Invoke();
            Debug.Log("Game Found");
            _ = ConnectToGameSession(gameFoundEvent.Data.ConnectionToken);
        };
    }
    #endregion

    #region GameSession

    private async Task ConnectToGameSession(string token)
    {
        try
        {
            var gameSession = ClientProvider.GetService<GameSession>();
            await gameSession.ConnectToGameSession(token);
            gameSession.OnAllPlayerReady += () =>
            {
                GameSession.OnAllPlayerReady.Invoke();
            };

            gameSession.OnShutdownReceived += () =>
            {
                GameSession.OnSessionLeaved.Invoke();
                GameSession.PostResultCancellationSource?.Cancel();
            };
            await gameSession.SetPlayerReady("");
            //await gameSession.EstablishDirectConnection(true);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred during ConnectToGameSession " + ex.Message);

        }
    }

    public void LeaveGameSession()
    {
        _ = LeaveGameSessionAsync();
    }

    private async Task LeaveGameSessionAsync()
    {
        var gameSession = ClientProvider.GetService<GameSession>();
        GameSession.PostResultCancellationSource?.Cancel();
        await gameSession.DisconnectFromGameSession();
        GameSession.OnSessionLeaved.Invoke();
    }

    public void PostResult()
    {
        _ = PostResultAsync();
    }

    public async Task PostResultAsync()
    {
        var gameSession = ClientProvider.GetService<GameSession>();
        EndGameDto dto = new EndGameDto();
        dto.Score = UnityEngine.Random.Range(0,10000);
        dto.LeaderboardName = "TesterUnity";
        GameSession.PostResultCancellationSource = new CancellationTokenSource();
        _lastGameResult = await gameSession.PostResult(dto, GameSession.PostResultCancellationSource.Token);
        GameSession.OnGameResultReceived.Invoke();
    }

    public void SetPlayerReady()
    {
        _ = SetPlayerReadyAsync();
    }

    private async Task SetPlayerReadyAsync()
    {
        try
        {
            var gameSession = ClientProvider.GetService<GameSession>();
            await gameSession.SetPlayerReady("");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during CreateParty : " + ex.Message);
        }
    }

    public void DisplayGameResult(Transform container)
    {
        int i = 0;
        foreach (var score in _lastGameResult.UsersScore)
        {
            var userScoreContainer = container.GetChild(i);
            var texts = userScoreContainer.GetComponentsInChildren<Text>();
            texts[0].text = score.Key;
            texts[1].text = score.Value;
            i++;
        }
    }

    #endregion

    #region Party

    public void CreateParty()
    {
        _ = CreatePartyAsync();
    }

    public async Task CreatePartyAsync()
    { 
        try
        {
            var party = ClientProvider.GetService<Party>();
            PartyRequestDto request = new PartyRequestDto();
            request.GameFinderName = "matchmakerdefault";
            request.PartySize = 1;
            request.StartOnlyIfPartyFull = false;
            var partyContainer = await party.CreateParty(request);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during CreateParty : " + ex.Message);
        }
    }

    public void LeaveParty()
    {
        _ = LeavePartyAsync();
    }

    private async Task LeavePartyAsync()
    {
        try
        {
            var party = ClientProvider.GetService<Party>();
            await party.LeaveParty();
            Party.OnPartyLeaved.Invoke();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during LeaveParty : " + ex.Message);
        }
    }

    private void DisplayPartyMembers(PartyUserDto[] users)
    {
        var myUserId = ClientProvider.GetService<AuthenticationService>().UserId;
        var texts = Party.PartyUserPanel.GetComponentsInChildren<Text>();
        for (int i = 0; i < texts.Length; i++)
        {
            if(i < users.Length)
            {
                texts[i].text = users[i].UserId;
                if (users[i].UserId == myUserId)
                {
                    Party.LeaderUI.SetActive(users[i].IsLeader);
                }
            }
            else
            {
                texts[i].text = "";
            }
        }
    }

    private void DisplayPartyInvitations(PartyInvitation[] invitations)
    {
        for (int i = 0; i < Party.PartyInvitationPanel.childCount; i++)
        {
            var child = Party.PartyInvitationPanel.GetChild(i);
            if(i < invitations.Length)
            {
                child.gameObject.SetActive(true);
                var textComponent = child.GetComponentInChildren<Text>();
                textComponent.text = invitations[i].UserId;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void AcceptPartyInvitation(Text text)
    {
        _ = AcceptPartyInvitationAsync(text);
    }

    public async Task AcceptPartyInvitationAsync(Text text)
    {
        try
        {
            var party = ClientProvider.GetService<Party>();
            List<PartyInvitation> invitations = party.Invitations.GetPartyInvitations();
            PartyInvitation currentInvitation = invitations.Find(element => element.UserId == text.text);
            if (currentInvitation != null)
            {
                party.Invitations.AnswerPartyInvitation(text.text, true);
                await party.JoinPartySceneByPlatformSessionId(currentInvitation.SceneId);
            }
            else
            {
                Debug.LogError("There is no invitation from : " + text.text);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during AcceptPartyInvitation : " + ex.Message);
        }
    }

    public void DeclinePartyInvitation(Text text)
    {
        try
        {
            var party = ClientProvider.GetService<Party>();
            party.Invitations.AnswerPartyInvitation(text.text, false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during DeclinePartyInvitation : " + ex.Message);
        }
    }

    public void SetPartyUserReady(bool ready)
    {
        _ = SetPartyUserReadyAsync(ready);
    }

    private async Task SetPartyUserReadyAsync(bool ready)
    {
        try
        {
            var party = ClientProvider.GetService<Party>();
            // this is to avoid status change when we leave the party and reset the toggle
            if (party != null)
            {
                await party.UpdatePlayerStatus(ready ? PartyUserStatus.Ready : PartyUserStatus.NotReady);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during SetPartyUserReady : " + ex.Message);
        }
    }

    public void InviteInParty()
    {
        _ = InviteInPartyAsync();
    }

    private async Task InviteInPartyAsync()
    {
        try
        {
            var party = ClientProvider.GetService<Party>();
            await party.SendInvitation(Party.UserIdTextField.text);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during InviteInParty : " + ex.Message);
        }
    }


    public void KickPlayerFromParty()
    {
        _ = KickPlayerFromPartyAsync();
    }

    private async Task KickPlayerFromPartyAsync()
    {
        try
        {
            var party = ClientProvider.GetService<Party>();
            var user = Party.UserIdTextField.text;
            if (await party.KickPlayer(user))
            {
                Debug.Log("Successfully kicked " + user);
            }
            else
            {
                Debug.Log("Failed to kick " + user);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during KickPlayerFromParty : " + ex.Message);
        }
    }

    public void PromoteLeader()
    {
        _ = PromoteLeaderAsync();
    }

    private async Task PromoteLeaderAsync()
    {
        try
        {
            var party = ClientProvider.GetService<Party>();
            if(await party.PromoteLeader(Party.UserIdTextField.text))
            {
                Party.LeaderUI.SetActive(false);
            }
            else
            {
                Debug.LogError("Failed to promote leader");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during PromoteLeader : " + ex.Message);
        }
    }

    private void RegisterPartyCallback()
    {
        var party = ClientProvider.GetService<Party>();

        party.OnPartyMembersUpdated += (users) =>
        {
            DisplayPartyMembers(users);
        };
        party.OnPartyKicked += () =>
        {
            LeaveParty();
        };
        party.OnPartyLeft += () =>
        {
            Party.OnPartyLeaved.Invoke();
        };
        party.OnPartyJoined += () =>
        {
            Party.OnPartyJoined.Invoke();
        };
        party.OnInvitationsUpdate += DisplayPartyInvitations;

    }
    #endregion

}
