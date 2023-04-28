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

public class MultiplayerManager : NetworkBehaviour
{
    private const int MAX_PLAYERS = 10;

    public Lobby joinedLobby { get; private set; }

    //The number of seconds until the host should send a heartbeat over the lobby.
    private float heartbeatTimer;

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
        NetworkManager.Singleton.StartHost();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += MultiplayerManager_ConnectoinApprovalCallback;
        //NetworkManager.Singleton.OnClientConnectedCallback += MultiplayerManager_OnClientConnectedCallback;

        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        //Decide if this client is allowed to connect to the host.
        //NetworkManager.Singleton.ConnectionApprovalCallback += MultiplayerManager_ConnectoinApprovalCallback;

        NetworkManager.Singleton.OnClientDisconnectCallback += MultiplayerManager_OnClientDisconnectCallback;

        NetworkManager.Singleton.StartClient();

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

    /// <summary>
    /// The delegate that determines if a player is allowed to connect to this game instance.
    /// </summary>
    private void MultiplayerManager_ConnectoinApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
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

    public async void JoinWithCode(string lobbyCode)
    {
        try
        {
            OnConnectingStarted?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            StartClient();

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

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        try
        {
            //Opens loading screen
            OnConnectingStarted?.Invoke(this, EventArgs.Empty);

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS, new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
            });

            GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.Lobby);

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

        NetworkManager.Singleton.ConnectionApprovalCallback -= MultiplayerManager_ConnectoinApprovalCallback;
        //NetworkManager.Singleton.OnClientConnectedCallback -= MultiplayerManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientConnectedCallback -= MultiplayerManager_OnClientDisconnectCallback;

        //If the player is in a lobby, remove them from it.
        if (joinedLobby != null)
        {
            try
            {
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
        HandleHeartbeat();
    }

    /// <summary>
    /// Lobbies will destroy themselves if there is no activity (called a heartbeat) in 30 seconds. This prevents that.
    /// </summary>
    private void HandleHeartbeat()
    {
        try
        {
            if (IsHost && joinedLobby != null)
            {
                heartbeatTimer -= Time.deltaTime;

                if (heartbeatTimer <= 0)
                {
                    float heartbeatTimerMax = 15f;
                    heartbeatTimer = heartbeatTimerMax;

                    LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
                }
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

    /*
    /// <summary>
    /// Code to run when a joins the game, including the host.
    /// </summary>
    private void MultiplayerManager_OnClientConnectedCallback(ulong clientID)
    {

    }


/*


    /*
try
{
//CreateAllocationAsync's parameter is the number of players, not including the host.
Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
//RelayServerData relayServerData1 = new RelayServerData();

NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

NetworkManager.Singleton.StartHost();
}
catch(RelayServiceException exception)
{
Debug.LogException(exception);
}
*/
}
