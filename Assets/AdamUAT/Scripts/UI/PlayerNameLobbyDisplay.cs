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

        GameManager.instance.multiplayerManager.UpdateLobby += UpdatePlayerListDisplay;
    }

    private void UpdatePlayerListDisplay(object sender, System.EventArgs e)
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
