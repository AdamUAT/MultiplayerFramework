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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
