using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using TMPro;

public class DisplayLobbyCode : UIObject
{
    private void OnEnable()
    {
        TextMeshProUGUI codeDisplay = GetComponentInChildren<TextMeshProUGUI>();

        if(codeDisplay != null)
        {
            Lobby lobby = GameManager.instance.multiplayerManager.joinedLobby;
            if (lobby != null)
            {
                codeDisplay.text = lobby.LobbyCode;
            }
            else
            {
                Debug.LogWarning("The lobby has not been created yet.");
            }
        }
        else
        {
            Debug.LogError("Could not find the codeDisplay.");
        }
    }
}
