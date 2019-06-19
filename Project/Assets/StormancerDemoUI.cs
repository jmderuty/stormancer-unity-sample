﻿using Stormancer;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using Stormancer.Plugins;
using UnityEngine.SceneManagement;
using System.Threading;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using Stormancer.Core;
using UnityEditor;

#pragma warning disable CS0618 // Type or member is obsolete
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
    public Text RoleText;
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
public class LeaderboardPropertyHolder
{
    public Transform LeaderboardPanel;
    public Button NextCursor;
    public Button PreviousCursor;
    public LeaderboardResult CurrentResult { get; set; }
}

public class StringMessage : MessageBase
{
    public string Message;

}

public class StormancerDemoUI : MonoBehaviour
{
    public InputField UserIdField;

    public AuthenticationPropertyHolder Authentication; 

    public GameFinderPropertyHolder GameFinder;

    public GameSessionPropertyHolder GameSession;

    public PartyPropertyHolder Party;
    private bool _invitationVisible = false;

    public LeaderboardPropertyHolder Leaderboard;

    private GameSessionResult _lastGameResult;
    private NetworkClient _netClient;
    private short _unityMessageId = 1000;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    { 
        if(ClientProvider.ClientId != 0)
        {
            var auth = ClientProvider.GetService<AuthenticationService>();
            UserIdField.text = auth.UserId;
        }
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
            RegisterPartyCallbacks();
            RegisterGameFinderCallbacks();
            RegisterGameSessionCallbacks();
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred on Login "+ex.Message);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("ON DESTROY");
        ClientProvider.CloseClient();
    }

    private void CheckConnectionState(GameConnectionStateCtx gameConnectionStateCtx)
    {
#if UNITY_EDITOR
        //this will avoid the UI to change when we stop playing in editor
        if(EditorApplication.isPlaying)
        {
#endif

            if (gameConnectionStateCtx.State == GameConnectionState.Authenticated)
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
#if UNITY_EDITOR
        }
#endif
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
    
    public void RegisterGameFinderCallbacks()
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

    private void RegisterGameSessionCallbacks()
    {
        var gameSession = ClientProvider.GetService<GameSession>();
        gameSession.OnAllPlayerReady += () =>
        {
            GameSession.OnAllPlayerReady.Invoke();
        };

        gameSession.OnShutdownReceived += () =>
        {
            GameSession.OnSessionLeaved.Invoke();
            GameSession.PostResultCancellationSource?.Cancel();
        };
        gameSession.OnRoleReceived += sessionParameters =>
        {
            if (sessionParameters.IsHost)
            {
                SetupServer(sessionParameters);
            }
            GameSession.RoleText.text = sessionParameters.IsHost ? "HOST" : "CLIENT";
        };

        gameSession.OnTunnelOpened += sessionParameters =>
        {
            SetupClient(sessionParameters);
        };

        gameSession.OnGameSessionConnectionChanged += state =>
        {
            if (state.State == ConnectionState.Disconnected)
            {
                ShutdownUnityConnection();
            }
        };
    }

    private async Task ConnectToGameSession(string token)
    {
        try
        {
            var gameSession = ClientProvider.GetService<GameSession>();
            await gameSession.ConnectToGameSession(token, true);
            await gameSession.SetPlayerReady("");
            await gameSession.EstablishDirectConnection();
        }
        catch (System.Exception ex) when (!(ex is OperationCanceledException))
        {
            Debug.LogError("An error occurred during ConnectToGameSession " + ex.Message);
        }
    }
    #region unityNet
    private void ShutdownUnityConnection()
    {
        if (_netClient == null)
        {
            NetworkServer.Shutdown(); 
        }
        else
        {
            _netClient.Disconnect();
            _netClient = null;
        }
    }

    // Server

    private void SetupServer(GameSessionConnectionParameters param)
    {
        
        if (!NetworkServer.active)
        {
            var config = ClientProvider.GetService<ClientConfiguration>();
            Debug.Log($"SetupServer on port {config.ServerGamePort}");
            NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnected);
            NetworkServer.RegisterHandler(_unityMessageId, OnClientMessage);
            NetworkServer.Listen(config.ServerGamePort);
        }
        else
        {
            Debug.LogError("server already setup");
        }
    }

    private void OnClientConnected(NetworkMessage netMessage)
    {
        // Do stuff when a client connects to this server

        // Send a thank you message to the client that just connected
        var messageContainer = new StringMessage();
        messageContainer.Message = "Thanks for joining!";

        // This sends a message to a specific client, using the connectionId
        NetworkServer.SendToClient(netMessage.conn.connectionId, _unityMessageId, messageContainer);

        // Send a message to all the clients connected
        messageContainer = new StringMessage();
        messageContainer.Message = "A new player has connected to the server";

        // Broadcast a message a to everyone connected
        NetworkServer.SendToAll(_unityMessageId, messageContainer);
    }

    private void OnClientDisconnected(NetworkMessage netMessage)
    {
        // Do stuff when a client disconnects
        // Send a message to all the clients connected
        var messageContainer = new StringMessage();
        messageContainer.Message = "A player has left the server";

        // Broadcast a message a to everyone connected
        NetworkServer.SendToAll(_unityMessageId, messageContainer);
        Debug.Log(messageContainer.Message);
    }

    private void OnClientMessage(NetworkMessage message)
    {
        var msg = message.ReadMessage<StringMessage>();
        Debug.Log($"Message received from client : {msg.Message}");
    }

    // CLIENT
    private void SetupClient(GameSessionConnectionParameters param)
    {
        Debug.Log("SetupClient");
        _netClient = new NetworkClient();
        _netClient.RegisterHandler(MsgType.Connect, OnConnected);
        _netClient.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        _netClient.RegisterHandler(_unityMessageId, OnMessageReceived);
        var endpointParts = param.Endpoint.Split(':');
        int port = Convert.ToInt32(endpointParts[1]);
        _netClient.Connect(endpointParts[0], port);
    }

    void OnConnected(NetworkMessage message)
    {
        Debug.Log("On unity network connected");
        // Do stuff when connected to the server

        StringMessage messageContainer = new StringMessage();
        messageContainer.Message = "Hello server!";

        // Say hi to the server when connected
        _netClient.Send(_unityMessageId, messageContainer);
    }

    void OnDisconnected(NetworkMessage message)
    {
        // Do stuff when disconnected to the server
        Debug.Log("Disconnected from unity server ");
    }

    // Message received from the server
    void OnMessageReceived(NetworkMessage netMessage)
    {
        // You can send any object that inherence from MessageBase
        // The client and server can be on different projects, as long as the MyNetworkMessage or the class you are using have the same implementation on both projects
        // The first thing we do is deserialize the message to our custom type
        var objectMessage = netMessage.ReadMessage<StringMessage>();

        Debug.Log("Message received from unity network: " + objectMessage.Message);
    }
    #endregion


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
        var party = ClientProvider.GetService<Party>();
        if (party.IsInParty)
        {
            Party.OnPartyJoined?.Invoke();
        }
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

    public void ToggleInvitationVisibility()
    {
        if(_invitationVisible)
        {
            HideInvitations();
        }
        else
        {
            ShowInvitations();
        }
    }

    private void ShowInvitations()
    {
        _invitationVisible = true;
        var animator = Party.PartyInvitationPanel.GetComponent<Animator>();
        animator.SetBool("ShowInvitations", true);
        var text = Party.PartyInvitationPanel.GetChild(4).GetComponentInChildren<Text>();
        text.text = ">";
    }

    private void HideInvitations()
    {
        _invitationVisible = false;
        var animator = Party.PartyInvitationPanel.GetComponent<Animator>();
        animator.SetBool("ShowInvitations", false);
        var text = Party.PartyInvitationPanel.GetChild(4).GetComponentInChildren<Text>();
        text.text = "<";
    }


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
        ShowInvitations();
        for (int i = 0; i < Party.PartyInvitationPanel.childCount-1; i++)
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
            if (party.IsInParty)
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

    private void RegisterPartyCallbacks()
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

    #region Leaderboard

    public void QueryLeaderboard()
    {
        _ = QueryLeaderboardAsync();
    }

    public async Task QueryLeaderboardAsync()
    {
        try
        {
            var query = new LeaderboardQuery();
            query.Size = 10;
            query.LeaderboardName = "TesterUnity";
            var leaderboard = ClientProvider.GetService<Leaderboard>();
            Leaderboard.CurrentResult = await leaderboard.Query(query);
            DisplayLeaderboard(Leaderboard.CurrentResult);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during QueryLeaderboard : " + ex.Message);
        }

    }

    public void QueryNextCursor()
    {
        _ = QueryNextCursorAsync();
    }

    public async Task QueryNextCursorAsync()
    {
        try
        {
            var leaderboard = ClientProvider.GetService<Leaderboard>();
            Leaderboard.CurrentResult = await leaderboard.Query(Leaderboard.CurrentResult.Next);
            DisplayLeaderboard(Leaderboard.CurrentResult);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during QueryNextCursor : " + ex.Message);
        }
    }

    public void QueryPreviousCursor()
    {
        _ = QueryPreviousCursorAsync();
    }

    public async Task QueryPreviousCursorAsync()
    {
        try
        {
            var leaderboard = ClientProvider.GetService<Leaderboard>();
            Leaderboard.CurrentResult = await leaderboard.Query(Leaderboard.CurrentResult.Previous);
            DisplayLeaderboard(Leaderboard.CurrentResult);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during QueryPreviousCursor : " + ex.Message);
        }
    }

    private void DisplayLeaderboard(LeaderboardResult result)
    {
        if(result.Next != null && result.Next != "")
        {
            Leaderboard.NextCursor.gameObject.SetActive(true);
        }
        else
        {
            Leaderboard.NextCursor.gameObject.SetActive(false);
        }

        if (result.Previous != null && result.Previous != "")
        {
            Leaderboard.PreviousCursor.gameObject.SetActive(true);
        }
        else
        {
            Leaderboard.PreviousCursor.gameObject.SetActive(false);
        }
        var myUserId = ClientProvider.GetService<AuthenticationService>().UserId;
        var ranks = Leaderboard.LeaderboardPanel.GetChild(0);
        var userIds = Leaderboard.LeaderboardPanel.GetChild(1);
        var scores = Leaderboard.LeaderboardPanel.GetChild(2);

        for (int i = 0; i < 10; i++)
        {
            if(i < result.Results.Count)
            {
                var res = result.Results[i];
                var rank = ranks.GetChild(i).GetComponent<Text>();
                var userId = userIds.GetChild(i).GetComponent<Text>();
                var score = scores.GetChild(i).GetComponent<Text>();
                if (myUserId == res.ScoreRecord.Id)
                {
                    rank.color = Color.red;
                    userId.color = Color.red;
                    score.color = Color.red;
                }
                else
                {
                    rank.color = Color.black;
                    userId.color = Color.black;
                    score.color = Color.black;
                }
                rank.text = ""+res.Ranking;
                userId.text = res.ScoreRecord.Id;
                score.text = ""+res.ScoreRecord.Score;
            }
            else
            {
                ranks.GetChild(i).GetComponent<Text>().text = "";
                userIds.GetChild(i).GetComponent<Text>().text = "";
                scores.GetChild(i).GetComponent<Text>().text = "";
            }
        }
    }
    #endregion

}

#pragma warning restore CS0618 // Type or member is obsolete