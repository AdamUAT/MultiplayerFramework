using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRandom : UIObject
{
    protected override void Awake()
    {
        base.Awake();

        Button joinButton = GetComponent<Button>();
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(() =>
            {
                GameManager.instance.multiplayerManager.QuickJoin();
            });
        }
        else
        {
            Debug.LogError("The joinButton was not found.");
        }
    }
}
