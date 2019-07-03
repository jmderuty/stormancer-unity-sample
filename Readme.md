# Sample  Unity

## Configuration:

First you will need to configure your stormancer client.
- Select your authenticationProvider and initialize it
- Get the configuration for your server application account (this will create a new one if there is none)
- Set the TaskGetAuthParameters of the configuration, this is how the authentication provider will find the connection information
- Set the server endpoints of the configuration
- Add all the plugin you will use in your game / application
- (optionnal) enable the DebugLogger to see stormancer logs in unity, do not set config.Logger otherwise
- Set the config in the ClientFactory with an Id that will be used to get the client corresponding to this configuration

Here is an example in `InitializeStormancerClient` in `StormancerDemoUI.cs`

```cs
#error this step is mendatory for you to configure, you can change the Authentication provider, the configuration account and the plugin used
var authenticationProvider = new RandomAuthenticationProvider();
authenticationProvider.Initialize();
var config = ClientConfiguration.ForAccount("sample-unity", "sample");
config.TaskGetAuthParameters = authenticationProvider.GetAuthArgs();
config.ServerEndpoints = new List<string>() { "http://gc3.stormancer.com" };
config.Plugins.Add(new AuthenticationPlugin());
config.Plugins.Add(new GameSessionPlugin());
config.Plugins.Add(new GameFinderPlugin());
config.Plugins.Add(new PartyPlugin());
config.Plugins.Add(new LeaderboardPlugin());
// This enable the log of stormancer in unity
config.Logger = DebugLogger.Instance;
ClientFactory.SetConfig(_clientId, () => { return config; });
```

The #error tag is here to tell the user to change those parameters, once you have changed it you can comment it.

## Serializer unity generation tool

To work efficiently, we need to generate Serializer for our data that will be send through internet. To do this we made a unity editor tool. If you have to modify the data that you will send with messages, you will need to generate the custom serializer for this data. If you want to add a new data structure, you will also have to generate the related serializer. One important point is that the tool will generate from the latest building version of your source. This mean that you will have to fix have a compiling version to generate the serializer. There is also a clear serializer option to remove all custom serializer. If you don’t generate a custom serializer for your data, it will be generated at runtime. You can find those tools in the "Stormancer" dropdown menu in Unity editor.


## Authentication plugin

- Login
- Auto reconnection
- Logout
- Get user Id


```cs
var client = ClientFactory.GetClient(_clientId);
var auth = client.DependencyResolver.Resolve<AuthenticationService>();
auth.OnGameConnectionStateChanged += CheckConnectionState;
await auth.Login();
```

[Click here for authentication service documentation](https://stormancer-docs.azurewebsites.net/Refs/stormancer.client.Unity.Plugins/html/class_stormancer_1_1_plugins_1_1_authentication_service.html)

## GameFinder plugin

- FindGame
- Cancel FindGame
```cs
var client = ClientFactory.GetClient(_clientId);
var gameFinder = client.DependencyResolver.Resolve<GameFinder>();
await gameFinder.FindGame("matchmakerdefault", "json", "{}");
```
GameFinder callbacks
```cs
var client = ClientFactory.GetClient(_clientId);
var gameFinder = client.DependencyResolver.Resolve<GameFinder>();
gameFinder.OnGameFinderStateChanged += statusChangedEvent =>
{
    // Game finder Status changed (Idle, Searching, Success, Failed, ...)
};
gameFinder.OnFindGameFailed += failedEvent =>
{
    // Game finder failed reason in failedEvent.Reason
};
gameFinder.OnGameFound += gameFoundEvent =>
{
    // Game Found with the data to connect to the game session
};
```

[Click here for GameFinder documentation](https://stormancer-docs.azurewebsites.net/Refs/stormancer.client.Unity.Plugins/html/class_stormancer_1_1_plugins_1_1_game_finder.html)

## GameSession plugin

- Connect to GameSession
- Leave GameSession
- Set player ready
- Peer to Peer connection with EstablishDirectConnection

```cs
var client = ClientFactory.GetClient(_clientId);
var gameSession = client.DependencyResolver.Resolve<GameSession>();
// Second parameter is useTunnel, if useTunnel is true, stormancer will be used as a tunnel for 
// other network system (eg: UNET). Else, you will directly use stormancer
await gameSession.ConnectToGameSession(token, true);
// You need to set the player Ready. The game session only starts when all player are ready
await gameSession.SetPlayerReady("");
// EstablishDirectConnection is to enable P2P connection.
await gameSession.EstablishDirectConnection();
```

GameSession callbacks

```cs
var client = ClientFactory.GetClient(_clientId);
var gameSession = client.DependencyResolver.Resolve<GameSession>();
// Triggers when all players are ready
gameSession.OnAllPlayerReady += () =>
{

};

// Triggers when the game session is shutting down
gameSession.OnShutdownReceived += () =>
{
    //Clean
};
// Triggers when you receive your role in the game session (HOST or CLIENT)
gameSession.OnRoleReceived += sessionParameters =>
{
    string role = (sessionParameters.IsHost ? "HOST" : "CLIENT");
    Debug.Log("OnRoleReceived : "+ role);
    if (sessionParameters.IsHost)
    {
        SetupUNetServer(sessionParameters);
    }
    GameSession.RoleText.text = role;
};

// Triggers when the game session connection status changed
gameSession.OnGameSessionConnectionChanged += state =>
{
    if (state.State == ConnectionState.Disconnected)
    {
        ShutdownUNetConnection();
    }
};

// Triggers when the P2PTunnel is open (only with P2P and tunnel enable)
gameSession.OnTunnelOpened += sessionParameters =>
{
    SetupUNetClient(sessionParameters);
};

// Triggers when the client connects to a scene
gameSession.OnConnectingToScene += scene =>
{
    scene.AddRoute("test", packet =>
    {
        Debug.Log($"Received a test message from peer {packet.Connection.Id}");
    }, MessageOriginFilter.Peer);

    scene.AddRoute("testbroadcast", packet =>
    {
        Debug.Log($"Received a test boardcast message from peer {packet.Connection.Id}");
    }, MessageOriginFilter.Peer);
};

// Triggers when a peer connects to me with P2P
gameSession.OnPeerConnected += peer =>
{
    gameSession.GetScene().Send(PeerFilter.MatchPeers(peer.Connection.Key), "test", stream => { });
    gameSession.GetScene().Send(PeerFilter.MatchAllP2P(), "testbroadcast", stream => { });
};
```
[Click here for GameSession documentation](https://stormancer-docs.azurewebsites.net/Refs/stormancer.client.Unity.Plugins/html/class_stormancer_1_1_plugins_1_1_game_session.html)

## Leaderboard plugin

- Query leaderboard (leaderboardName, query size, start index, skip number, ...)
- Query leaderboard cursor (Previous / Next) to navigate through the leaderboard

```cs
var query = new LeaderboardQuery();
query.Size = 10;
query.LeaderboardName = "TesterUnity";
var client = ClientFactory.GetClient(_clientId);
var leaderboard = client.DependencyResolver.Resolve<Leaderboard>();
var leaderboardResult = await leaderboard.Query(query);
```

[Click here for Leaderboard documentation](https://stormancer-docs.azurewebsites.net/Refs/stormancer.client.Unity.Plugins/html/class_stormancer_1_1_plugins_1_1_leaderboard.html)

## Party plugin

- Create party
- Send party invitations
- Answer party invitation (Accept / Deny)
- Kick player from party (only for the party leader)
- Promote player to party leader (only for the party leader)

```cs
var client = ClientFactory.GetClient(_clientId);
var party = client.DependencyResolver.Resolve<Party>();
PartyRequestDto request = new PartyRequestDto();
request.GameFinderName = "matchmakerdefault";
request.PartySize = 2;
request.StartOnlyIfPartyFull = false;
var partyContainer = await party.CreateParty(request);
```
Party callbacks
```cs
var client = ClientFactory.GetClient(_clientId);
var party = client.DependencyResolver.Resolve<Party>();

// Triggered when a new player enters the party or when one leaves or is kicked
party.OnPartyMembersUpdated += (users) =>
{
    DisplayPartyMembers(users);
};
// Triggers when you are kicked from a party
party.OnPartyKicked += () =>
{
    LeaveParty();
};
// Triggers when you leave a party
party.OnPartyLeft += () =>
{
    Party.OnPartyLeaved.Invoke();
};
//Triggers when you join a party
party.OnPartyJoined += () =>
{
    Party.OnPartyJoined.Invoke();
};
// Triggers when you receive a party invitation
party.OnInvitationsUpdate += DisplayPartyInvitations;
```


[Click here for Party documentation](https://stormancer-docs.azurewebsites.net/Refs/stormancer.client.Unity.Plugins/html/class_stormancer_1_1_plugins_1_1_party.html)











