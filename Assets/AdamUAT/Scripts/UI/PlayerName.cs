using System.Collections;
using System.Collections.Generic;
using TMPro;
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
            playerName.text = GameManager.instance.multiplayerManager.playerName;
            playerName.onValueChanged.AddListener((string newText) =>
            {
                GameManager.instance.multiplayerManager.playerName = newText;
            });
        }
        else
        {
            Debug.LogError("The input field playerName could not be found.");
        }
    }
}
