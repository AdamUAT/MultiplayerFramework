using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;
using Unity.Services.Lobbies.Models;
using UnityEngine.UIElements;

public class PlayerController : Controller
{

    #region PlayerData
    /// <summary>
    /// The name of this player. This value is updated from MultiplayerManager. During an active multiplayer session, this value should be used.
    /// </summary>
    public NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>("DefaultName");
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNameServerRpc(string newName)
    {
        playerName.Value = newName;
    }
    public string GetPlayerName()
    {
        return (playerName.Value.ToString());
    }

    //The bool to tell if the player is ready or not. This is only used in the lobby.
    private NetworkVariable<bool> isPlayerReady = new NetworkVariable<bool>(false);
    public bool GetIsPlayerReady()
    {
        return(isPlayerReady.Value);
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetIsPlayerReadyServerRpc(bool _playerReady)
    {
        isPlayerReady.Value = _playerReady;
    }

    #endregion PlayerData

    public event EventHandler Interact;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        GameManager.instance.players.Add(this);

        //This code runs when the player joins the lobby.
        if (GameManager.instance.multiplayerManager.joinedLobby != null)
        {
            //Checks to see if the spawned Network object is associated with the local client.
            if (IsLocalPlayer)
            {
                //Set's the name of this player.
                SetPlayerNameServerRpc(GameManager.instance.multiplayerManager.GetLocalPlayerName(true));
            }

            //A specific event is called when the PlayerController is spawned, because the PlayerController isn't spawned in the same frame as when a client joins a server, so OnClientConnected event is too early.
            playerName.OnValueChanged = (FixedString64Bytes previousValue, FixedString64Bytes newValue) =>
            {
                GameManager.instance.multiplayerManager.CallUpdateLobbyEvent();
            };

            isPlayerReady.OnValueChanged = (bool previousValue, bool newValue) =>
            {
                GameManager.instance.multiplayerManager.CheckLobbyStateServerRpc();
            };
            //Also check it immediatly when a player connects.
            GameManager.instance.multiplayerManager.CheckLobbyStateServerRpc();

            //Updates as soon as it spawns.
            GameManager.instance.multiplayerManager.CallUpdateLobbyEvent();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameManager.instance.players.Remove(this);

        //This code runs when the player leaves the lobby, so it only runs if the player is in a lobby.
        if (GameManager.instance.multiplayerManager.joinedLobby != null)
        {
            //Updates the lobby after this is destroyed, removing it from the list.
            GameManager.instance.multiplayerManager.CallUpdateLobbyEvent();

            //Also check the ready after a player disconnects.
            GameManager.instance.multiplayerManager.CheckLobbyStateServerRpc();
        }
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void DoClientUpdate()
    {
        if (!GameManager.instance.IsPaused)
        {
            ProcessInputs();
        }
        else
        { 
            ProcessPausedInputs();
        }
    }

    private void ProcessInputs()
    {
        //Only call the ServerRpcs if the player is putting in input.
        if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            MovePawnServerRpc(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        if(Input.GetAxis("Mouse X") != 0)
        {
            RotatePawnServerRpc(Input.GetAxis("Mouse X"));
        }
        if(Input.GetKeyDown(GameManager.instance.inputManager.interactKey))
        {
            Interact?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ProcessPausedInputs()
    {            
        //The inputs that are allowed when paused but not allowed when dead.
        if (GameManager.instance.gameStateManager.currentGameState != GameStateManager.GameState.Dead)
        {

        }
    }

        /// <summary>
        /// Spawns this pawn on the server.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
    public void SpawnPawnOnServerRpc(PawnPrefabsManager.Pawns pawnToSpawn, Vector3 position, Quaternion rotation)
    {
        Pawn = Instantiate(GameManager.instance.pawnPrefabsManager.pawnPrefabsDictionary.GetValueOrDefault(pawnToSpawn), position, rotation).GetComponent<Pawn>();
        NetworkObject networkObject = Pawn.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
        else
        {
            Debug.LogError("No NetworkObject found on pawn. It has becom desynchronized from the network.");
        }

        //Passes in the Player Controller via networkObject reference.
        Pawn.AssignReferencesClientRpc(GetComponent<NetworkObject>());
        PlayerHealth playerHealth = Health as PlayerHealth;
        if (playerHealth != null)
        {
            playerHealth.CreateHealthUIClientRpc();
        }
        else
        {
            Debug.LogError("The health component on the playerController was not a playerHealth!");
        }
    }

    /// <summary>
    /// Switches which pawn this controller is using.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void SwitchPawnOnServerRpc(PawnPrefabsManager.Pawns pawnToSpawn)
    {
        Vector3 position = Pawn.transform.position;
        Quaternion rotation = Pawn.transform.rotation;

        NetworkObject networkObject = Pawn.GetComponent<NetworkObject>();

        //Despawn the old pawn from the network.
        if (networkObject != null)
        {
            networkObject.Despawn();
        }
        else
        {
            Debug.LogError("No NetworkObject found on pawn. It has becom desynchronized from the network.");
        }

        Pawn = Instantiate(GameManager.instance.pawnPrefabsManager.pawnPrefabsDictionary.GetValueOrDefault(pawnToSpawn), position, Quaternion.identity).GetComponent<Pawn>();

        //Spawn the new pawn on the network.
        networkObject = Pawn.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
        else
        {
            Debug.LogError("No NetworkObject found on pawn. It has becom desynchronized from the network.");
        }

        Pawn.AssignReferencesClientRpc(GetComponent<NetworkObject>());

        //Sets initial rotation. This is done from the pawn itself, since some pawns rotate differently.
        if (Pawn.Movement != null)
        {
            Pawn.Movement.Rotate(rotation.eulerAngles);
        }
        else
        {
            Debug.LogError("Pawn's movement component is null.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MovePawnServerRpc(float horizontal, float vertical)
    {
        if (Pawn != null)
        {
            if (Pawn.Movement != null)
            {
                Pawn.Movement.Move(new Vector2(horizontal, vertical));
            }
            else
            {
                Debug.LogError("Pawn's movement component is null.");
            }
        }
        else
        {
            Debug.LogError("Player Controller has no associated pawn.");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RotatePawnServerRpc(float rotationAmount)
    {
        if(Pawn != null)
        {
            if (Pawn.Movement != null)
            {
                Pawn.Movement.Rotate(new Vector3(0, rotationAmount, 0));
            }
            else
            {
                Debug.LogError("Pawn's movement component is null.");
            }
        }
        else
        {
            Debug.LogError("Player Controller has no associated pawn.");
        }
    }
}
