using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class DisplayLobbyPassword : UIObject
{
    private void OnEnable()
    {
        TextMeshProUGUI passwordDisplay = GetComponentInChildren<TextMeshProUGUI>();

        if (passwordDisplay != null)
        {
            passwordDisplay.text = GameManager.instance.multiplayerManager.storedPassword;

            Debug.Log(GameManager.instance.multiplayerManager.storedPassword);
        }
        else
        {
            Debug.LogError("Could not find the codeDisplay.");
        }
    }
}
