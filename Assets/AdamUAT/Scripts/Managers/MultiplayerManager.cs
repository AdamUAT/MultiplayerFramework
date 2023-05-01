using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using System;
using System.Threading.Tasks;
using System.Text;
using Unity.Collections;

public class MultiplayerManager : NetworkBehaviour
{
    private const int MAX_PLAYERS = 10;
    private const int MIN_PLAYERS = 1; //How many players have to be in a lobby before they are all allowed to ready.
    private const int LOBBY_READY_DELAY = 5;

    //This join code will almost never be used.
    private const string RELAY_JOIN_CODE = "RelayJoinCode.";
    public Lobby joinedLobby { get; private set; }

    //The number of seconds until the host should send a heartbeat over the lobby.
    private float heartbeatTimer;

    public bool isMultiplayer { get; private set; } = false;

    /// <summary>
    /// The name of the player. Currently, this is set up so it's a local variable set by the user.
    /// Later this could change if the game requires users to set up accounts.
    /// </summary>
    private string localPlayerName;
    /// <summary>
    /// Gets the player name for the local client.
    /// </summary>
    /// <param name="isHard">If it should ignore the check for the PlayerController.</param>
    public string GetLocalPlayerName(bool isHard = false)
    {
        NetworkClient localClient = NetworkManager.Singleton.LocalClient;
        if (localClient != null)
        {
            NetworkObject player = localClient.PlayerObject;
            if (!isHard && player != null)
            {
                //If the player exists, return it's name.
                return player.GetComponent<PlayerController>().playerName.ToString();
            }
        }

        return localPlayerName;
    }
    public void SetLocalPlayerName(string newName)
    {
        NetworkClient localClient = NetworkManager.Singleton.LocalClient;
        if(localClient != null)
        {
            NetworkObject player = localClient.PlayerObject;
            if (player != null)
            {
                //If the player exists, return it's name.
                player.GetComponent<PlayerController>().SetPlayerNameServerRpc(newName);
            }
        }

        localPlayerName = newName;
    }

    public event EventHandler OnConnectingStarted;
    public event EventHandler OnConnectingFinished;
    public event EventHandler OnDisconnectingStarted;
    public event EventHandler OnDisconnectingFinished;
    //public event EventHandler OnCreateLobbyFailed;
    //public event EventHandler OnJoinRandomLobbyFailed;
    //public event EventHandler OnJoinSpecificLobbyFailed;
    public event EventHandler UpdateLobby;
    public delegate void ShowLobbyMessage(string message);
    public ShowLobbyMessage showLobbyMessage;
    public Action<int> UpdateTimer;
    private bool isLobbyReady = false;
    private float lobbyReadyDelayTimer;
    public string storedPassword { get; private set; }

    //Do all the one-time code relay stuff that allows it to work
    //multiplayerManager.PrimeRelay();
    public async void PrimeRelay()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            //Allows the users to be differentiated.
            InitializationOptions options = new InitializationOptions();
            options.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(options);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    /// <summary>
    /// The current protocol of the transport is not set to Relay by default, so this will just run a local host and does not need internet connection.
    /// </summary>
    public void StartSinglePlayer()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = SinglePlayer_ConenctionApprovalCallback;

        NetworkManager.Singleton.StartHost();

        //If it doesn't change scenes on the network, then the player prefab will be deleted.
        GameManager.instance.sceneManager.ChangeSceneNetwork(CustomSceneManager.Scenes.Gameplay);

        isMultiplayer = false;
    }

    /// <summary>
    /// The connection approval callback must be made, even on singleplayer, because that determines if the player prefab is spawned or not.
    /// </summary>
    private void SinglePlayer_ConenctionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        //Only approve if the game is not in the lobby and the connecting client is the server.
        if (joinedLobby == null && connectionApprovalRequest.ClientNetworkId == NetworkManager.ServerClientId)
        {
            connectionApprovalResponse.Approved = true;
            connectionApprovalResponse.CreatePlayerObject = true;
        }
        else
        {
            connectionApprovalResponse.Approved = false;
        }
    }
    
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = MultiplayerManager_ConnectionApprovalCallback;
        //NetworkManager.Singleton.OnClientConnectedCallback += MultiplayerManager_OnClientConnectedCallback;



        NetworkManager.Singleton.StartHost();

        isMultiplayer = true;
    }

    public void StartClient()
    {
        //Decide if this client is allowed to connect to the host.
        //NetworkManager.Singleton.ConnectionApprovalCallback += MultiplayerManager_ConnectoinApprovalCallback;

        NetworkManager.Singleton.OnClientDisconnectCallback += MultiplayerManager_OnClientDisconnectCallback;

        NetworkManager.Singleton.StartClient();

        isMultiplayer = true;

        /*
        //Only add listener on the server
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }*/
    }

    public async void QuickJoin()
    {
        try
        {
            OnConnectingStarted?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            //Connect to relay.
            string relayJoinCode = joinedLobby.Data[RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            StartClient();

            GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Lobby);

            OnConnectingFinished?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogException(exception);

            //Get rid of the connecting text and tell the player that a lobby was not created.
            OnConnectingFinished?.Invoke(this, EventArgs.Empty);
            showLobbyMessage("Failed to join a lobby.");
        }
    }

    public async void JoinWithCode(string lobbyCode, string password = "")
    {
        try
        {
            OnConnectingStarted?.Invoke(this, EventArgs.Empty);

            //If the user doesn't provide a password, do not try to pass one in.
            if (password == "" || password == null)
            {
                joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            }
            else
            {
                joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions
                {
                    Password = password
                });
            }

            //Joins the relay.
            string relayJoinCode = joinedLobby.Data[RELAY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

            //This is what passes the password to the server.
            //NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(password);

            StartClient();

            //Sets the password the user put in as the password to display in the lobby.
            storedPassword = password;

            GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Lobby);

            OnConnectingFinished?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogException(exception);

            //Get rid of the connecting text and tell the player that the join failed.
            OnConnectingFinished?.Invoke(this, EventArgs.Empty);
            showLobbyMessage("Failed to join a lobby.");
        }
        catch(ArgumentNullException exception)
        {
            Debug.LogException(exception);

            //Get rid of the connecting text and tell the player that the join failed.
            OnConnectingFinished?.Invoke(this, EventArgs.Empty);
            showLobbyMessage("Failed to join a lobby.");
        }
    }

    public async void JoinRelay()
    {
        try
        {
            string relayJoinCode = joinedLobby.Data[RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        }
        catch(RelayServiceException exception)
        {
            Debug.LogException(exception);
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate, string password = "")
    {
        try
        {
            //Opens loading screen
            OnConnectingStarted?.Invoke(this, EventArgs.Empty);

            //Checks to see if this lobby should have a password or not.
            CreateLobbyOptions createLobbyOptions;
            if (!isPrivate || password == "" || password == null)
            {
                createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                };
            }
            else
            {
                createLobbyOptions = new CreateLobbyOptions
                {
                    IsPrivate = true,
                    Password = password
                };
            }

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS, createLobbyOptions);


            //Sets the password the user put in as the password to display in the lobby.
            storedPassword = password;

            GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Lobby);


            //CreateAllocationAsync's parameter is the number of players, not including the host.
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MAX_PLAYERS - 1);

            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                    { RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                 }
            });

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();


            StartHost();

            //Closes loading screen
            OnConnectingFinished?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogException(exception);

            //Get rid of the connecting text and tell the player that a lobby was not created.
            OnConnectingFinished?.Invoke(this, EventArgs.Empty);
            showLobbyMessage("Failed to create a lobby.");
            //OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
        }
        catch (RelayServiceException exception)
        {
            Debug.LogException(exception);
        }
    }

    /// <summary>
    /// Closes the lobby so no one else can join.
    /// </summary>
    public async void DeleteLobby()
    {
        try
        {
            //If the lobby is active
            if (joinedLobby != null)
            {

                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogException(exception);
        }
    }

    /// <summary>
    /// Disconnects the player from all netcode stuff.
    /// </summary>
    public async void Disconnect()
    {
        OnDisconnectingStarted?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.Shutdown();

        NetworkManager.Singleton.ConnectionApprovalCallback -= MultiplayerManager_ConnectionApprovalCallback;
        //NetworkManager.Singleton.OnClientConnectedCallback -= MultiplayerManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= MultiplayerManager_OnClientDisconnectCallback;

        //If the player is in a lobby, remove them from it.
        if (joinedLobby != null)
        {
            try
            {
                if(isLobbyReady)
                {
                    isLobbyReady = false;
                    UpdateTimer(-1);
                }

                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                //If the host is the one that left the lobby, then delete the lobby.
                if(IsHost)
                {
                    await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
                }
            }
            catch (LobbyServiceException exception)
            {
                Debug.LogException(exception);
            }

            joinedLobby = null;
        }

        //After the player has been disconnected, then change the game state to HostOrJoin. This works in both the Lobby and during GamePlay.
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.HostOrJoin);

        OnDisconnectingFinished?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        //Only do the heartbeat in a lobby.
        if (IsHost && joinedLobby != null)
        {
            HandleHeartbeat();
        }

        //Takes care of the lobby delay timer.
        if(isLobbyReady)
        {
            lobbyReadyDelayTimer -= Time.deltaTime;
            //The timer stops at -1. This is an illusion given to the players, allowing them to see a few frames of the timer at 0 before it switches scenes.
            if(lobbyReadyDelayTimer > -1)
            {
                UpdateTimer((int)Mathf.Ceil(lobbyReadyDelayTimer));
            }
            else
            {
                //Changes the scene.
                GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Gameplay);
                isLobbyReady = false; //Stops the timer.
            }
        }
    }

    /// <summary>
    /// Lobbies will destroy themselves if there is no activity (called a heartbeat) in 30 seconds. This prevents that.
    /// </summary>
    private void HandleHeartbeat()
    {
        try
        {
            heartbeatTimer -= Time.deltaTime;

            if (heartbeatTimer <= 0)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
        catch (LobbyServiceException exception)
        {
            Debug.LogException(exception);
        }
    }

    /// <summary>
    /// Allows the PlayerController to call the UpdateLobby event.
    /// </summary>
    public void CallUpdateLobbyEvent()
    {
        if(GameManager.instance.gameStateManager.currentGameState == GameStateManager.GameState.Lobby)
        {
            UpdateLobby?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Debug.LogWarning("Cannot update the lobby since it isn't the active state.");
        }
    }

    private void MultiplayerManager_OnClientDisconnectCallback(ulong clientID)
    {
        //Check if the host is disconnecting, and if so, trigger the disconnection.
        if (clientID == NetworkManager.ServerClientId)
        {
            //DisconnectAllClientRpc();
            showLobbyMessage("The host has disconnected.");

            Disconnect();
        }
    }
    
    /// <summary>
    /// The delegate that determines if a player is allowed to connect to this game instance.
    /// </summary>
    private void MultiplayerManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        //Only approve if the game is still in the lobby. This denies late joins.
        if (GameManager.instance.gameStateManager.currentGameState == GameStateManager.GameState.Lobby)
        {
            connectionApprovalResponse.Approved = true;
            connectionApprovalResponse.CreatePlayerObject = true;
        }
        else
        {
            connectionApprovalResponse.Approved = false;
        }
    }

    /// <summary>
    /// Checks if all players have readied up, and if so, starts a timer, or stops the timer.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void CheckLobbyStateServerRpc()
    {
        //Checks if there's enough players to be ready.
        if(GameManager.instance.players.Count < MIN_PLAYERS)
        {            
            //Cancles the timer if it was running.
            UpdateDelayTimerClientRpc(false);

            return;
        }

        //Checks if all player's are ready.
        foreach(PlayerController player in GameManager.instance.players)
        {
            //If any of the player's are not ready, then it stops.
            if(!player.GetIsPlayerReady())
            {
                //Cancles the timer if it was running.
                UpdateDelayTimerClientRpc(false);

                return;
            }
        }

        //If it reaches to hear, then the lobby is allowed to change.
        UpdateDelayTimerClientRpc(true);
    }

    [ClientRpc]
    public void UpdateDelayTimerClientRpc(bool isReady)
    {
        isLobbyReady = isReady;

        //Cancles the timer.
        if(isReady)
        {
            lobbyReadyDelayTimer = LOBBY_READY_DELAY;
        }
        else
        {
            UpdateTimer(-1);
        }
    }

    /// <summary>
    /// Allows other scripts to access whether or not they are part of the host or client.
    /// </summary>
    public bool IsClientHost()
    {
        return (IsHost);
    }

    /// <summary>
    /// The clientRpc that tells all the clients to update their player input. 
    /// This syncronizes all of them with the server and prevents the host from having an advantage.
    /// </summary>
    [ClientRpc]
    public void UpdatePlayersClientRpc()
    {
        foreach (PlayerController player in GameManager.instance.players)
        {
            //Checks to see if this is the playerController that belongs to this client, and if it isn't the lobby or end.
            if (player.IsLocalPlayer && GameManager.instance.gameStateManager.currentGameState == GameStateManager.GameState.Gameplay)
            {
                player.DoClientUpdate();
            }
        }
    }

    /// <summary>
    /// Disables all the UI except the one that should be showing on all clients.
    /// </summary>
    [ClientRpc]
    public void InitializeUIClientRpc()
    {
        GameManager.instance.uiManager.DisableAllUIObjects();

        GameManager.instance.uiManager.EnableUIObjectsWithGameState(GameManager.instance.gameStateManager.currentGameState);
    }
    /*
    /// <summary>
    /// Code to run when a joins the game, including the host.
    /// </summary>
    private void MultiplayerManager_OnClientConnectedCallback(ulong clientID)
    {

    }


*/
}
