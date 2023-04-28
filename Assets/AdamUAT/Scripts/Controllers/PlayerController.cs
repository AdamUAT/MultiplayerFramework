using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

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

    #endregion PlayerData

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.players.Add(this);

        //Checks to see if the spawned Network object is associated with the local client.
        if (GetComponent<NetworkObject>() == NetworkManager.Singleton.LocalClient.PlayerObject)
        {        
            //Set's the name of this player.
            SetPlayerNameServerRpc(GameManager.instance.multiplayerManager.GetLocalPlayerName(true));
        }

        //A specific event is called when the PlayerController is spawned, because the PlayerController isn't spawned in the same frame as when a client joins a server, so OnClientConnected event is too early.
        playerName.OnValueChanged += (FixedString64Bytes previousValue, FixedString64Bytes newValue) =>
        {
            GameManager.instance.multiplayerManager.CallUpdateLobbyEvent();
        };

        //Updates as soon as it spawns.
        GameManager.instance.multiplayerManager.CallUpdateLobbyEvent();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameManager.instance.players.Remove(this);

        //Updates the lobby after this is destroyed, removing it from the list.
        GameManager.instance.multiplayerManager.CallUpdateLobbyEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
