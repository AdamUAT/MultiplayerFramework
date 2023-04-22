using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameLobbyDisplay : UIObject
{
    protected override void Awake()
    {
        base.Awake();

        NetworkManager.Singleton.OnClientConnectedCallback += UpdatePlayerListDisplay;
        NetworkManager.Singleton.OnClientDisconnectCallback += UpdatePlayerListDisplay;
    }

    private void UpdatePlayerListDisplay(ulong obj)
    {
        TextMeshProUGUI playerListDisplay = GetComponent<TextMeshProUGUI>();
        if(playerListDisplay != null)
        {
            //Clears the list.
            playerListDisplay.text = "";

            //Gets every single player on the clients.
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                PlayerController player = client.PlayerObject.GetComponent<PlayerController>();

                //Outputs the player's name to a new line.
                playerListDisplay.text += player.GetName() + "\n";
            }
        }
        else
        {
            Debug.LogError("The playerListDisplay was not found.");
        }
    }
}
