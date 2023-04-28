using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinWithCode : UIObject
{
    [SerializeField]
    private TMP_InputField joinCode;
    [SerializeField]
    private TMP_InputField joinPassword;

    protected override void Awake()
    {
        base.Awake();

        Button joinButton = GetComponent<Button>();
        if(joinButton != null)
        {
            if (joinCode != null)
            {
                if (joinPassword != null)
                {
                    joinButton.onClick.AddListener(() =>
                    {
                        GameManager.instance.multiplayerManager.JoinWithCode(joinCode.text, joinPassword.text);
                    });
                }
                else
                {
                    Debug.LogError("The password input field is null.");
                }
            }
            else
            {
                Debug.LogError("The code input field is null.");
            }
        }
        else
        {
            Debug.LogError("The joinButton was not found.");
        }
    }
}
