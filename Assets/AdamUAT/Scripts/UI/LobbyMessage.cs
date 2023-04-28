using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyMessage : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI messageText;
    [SerializeField]
    private Button acknowledgeButton;

    private void Awake()
    {
        if(messageText != null)
        {
            //Whenever multiplayerManager calls showLobbyMessage, this recognizes it and changes the text to whatever was passed into it.
            GameManager.instance.multiplayerManager.showLobbyMessage = (string message) => { gameObject.SetActive(true); messageText.text = message; };
        }
        else
        {
            Debug.LogError("No text element component on LobbyMessage GameObject.");
        }

        if(acknowledgeButton != null)
        {
            acknowledgeButton.onClick.AddListener(() => { gameObject.SetActive(false); });
        }
        else
        {
            Debug.LogError("No button component on LobbyMessage GameObject.");
        }

        gameObject.SetActive(false);
    }
}
