using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyReadyToggle : UIObject
{
    protected override void Awake()
    {
        base.Awake();

        Toggle readyToggle = GetComponentInChildren<Toggle>();
        if (readyToggle != null)
        {
            readyToggle.onValueChanged.AddListener((bool value) =>
            {
                NetworkClient localClient = NetworkManager.Singleton.LocalClient;
                if(localClient != null)
                {
                    localClient.PlayerObject.GetComponent<PlayerController>().SetIsPlayerReadyServerRpc(value);
                }
                else
                {
                    Debug.LogWarning("Could not update the ready status of the player because the NetworkManager is not connected.");
                }
            });
        }
        else
        {
            Debug.LogError("The ready toggle was not found.");
        }
    }

    private void OnEnable()
    {
        //Starts the ready toggle at off each time the player joins a new lobby.
        Toggle readyToggle = GetComponentInChildren<Toggle>();
        if (readyToggle != null)
        {
            readyToggle.isOn = false;
        }
    }
}
