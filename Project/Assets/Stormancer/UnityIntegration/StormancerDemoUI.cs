using Stormancer;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.UI;
using Stormancer.Plugins;
using UnityEngine.SceneManagement;
using System.Threading;
using System;

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

public class StormancerDemoUI : MonoBehaviour
{
    public AuthenticationPropertyHolder Authentication; 

    public GameFinderPropertyHolder GameFinder;

    public GameSessionPropertyHolder GameSession;

    private GameSessionResult _lastGameResult;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    #region Authentication

    public void Login()
    {
        _ = ConnectPlayer();
    }

    async Task ConnectPlayer()
    {
        try
        {
            var auth = ClientProvider.GetService<AuthenticationService>();
            auth.OnGameConnectionStateChanged += CheckConnectionState;
            await auth.Login();
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private void OnDestroy()
    {
        ClientProvider.CloseClient();
    }

    void CheckConnectionState(GameConnectionStateCtx gameConnectionStateCtx)
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
            await gameFinder.FindGame("matchmakerdefault", "json", "{}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred during find match " + ex.Message);
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
            Debug.LogError("An error occurred during find match " + ex.Message);
        }
    }
    #endregion

    #region GameSession

    private async Task ConnectToGameSession(string token)
    {
        var gameSession = ClientProvider.GetService<GameSession>();
        await gameSession.ConnectToGameSession(token);
        _ = gameSession.SetPlayerReady("");
        //await gameSession.EstablishDirectConnection(true);
        gameSession.OnAllPlayerReady += () =>
        {
            GameSession.OnAllPlayerReady.Invoke();
        };

        gameSession.OnShutdownReceived += () =>
        {
            GameSession.OnSessionLeaved.Invoke();
            GameSession.PostResultCancellationSource?.Cancel();
        };
    }

    public void LeaveGameSession()
    {
        _ = LeaveGameSessionAsync();
    }

    public async Task LeaveGameSessionAsync()
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
        try
        {
            _lastGameResult = await gameSession.PostResult(dto, GameSession.PostResultCancellationSource.Token);
            GameSession.OnGameResultReceived.Invoke();
        }
        catch (Exception)
        {
        	
        }
    }

    public void SetPlayerReady()
    {
        var gameSession = ClientProvider.GetService<GameSession>();
        _ = gameSession.SetPlayerReady("");
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

}
