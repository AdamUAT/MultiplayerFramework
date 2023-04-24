using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : Controller
{

    #region PlayerData
    public string GetName()
    {
        return GameManager.instance.multiplayerManager.playerName;
    }
    #endregion PlayerData

    // Start is called before the first frame update
    void Start()
    {
        //A specific event is called when the PlayerController is spawned, because the PlayerController isn't spawned in the same frame as when a client joins a server, so OnClientConnected event is too early.
        GameManager.instance.multiplayerManager.CallUpdateLobbyEvent();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
