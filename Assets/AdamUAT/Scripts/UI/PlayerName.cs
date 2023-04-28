using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : UIObject
{
    protected override void Awake()
    {
        base.Awake();

        TMP_InputField playerName = GetComponent<TMP_InputField>();

        if(playerName != null)
        {
            playerName.onValueChanged.AddListener((string newText) =>
            {
                //Only is called if the lobby is active. (OnEnable's initialization was causing errors.)
                //This also only runs on clients, but since there's no dedicated server, instead using hosts, this shouldn't be a problem.
                if (NetworkManager.Singleton.IsClient)
                {
                    GameManager.instance.multiplayerManager.SetLocalPlayerName(newText);
                }
            });
        }
        else
        {
            Debug.LogError("The input field playerName could not be found.");
        }
    }

    private void OnEnable()
    {
        TMP_InputField playerName = GetComponent<TMP_InputField>();

        if (playerName != null)
        {
            playerName.text = GameManager.instance.multiplayerManager.GetLocalPlayerName();
        }
        else
        {
            Debug.LogError("The input field playerName could not be found.");
        }
    }
}
