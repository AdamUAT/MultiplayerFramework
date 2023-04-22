using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using TMPro;

public class DisplayLobbyName : UIObject
{
    private void OnEnable()
    {
        TextMeshProUGUI nameDisplay = GetComponentInChildren<TextMeshProUGUI>();

        if(nameDisplay != null)
        {
            Lobby lobby = GameManager.instance.multiplayerManager.joinedLobby;
            if(lobby != null)
            { 
                nameDisplay.text = lobby.Name;
            }
            else
            {
                Debug.LogWarning("The lobby has not been created yet.");
            }
        }
        else
        {
            Debug.LogError("Could not find the nameDisplay.");
        }
    }
}