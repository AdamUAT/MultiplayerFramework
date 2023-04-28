using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateMatch : UIObject
{
    [SerializeField]
    private Toggle isLobbyPublic;
    [SerializeField]
    private TMP_InputField lobbyName;
    [SerializeField]
    private TMP_InputField lobbyPassword;

    protected override void Awake()
    {
        base.Awake();

        Button createMatchButton = GetComponent<Button>();

        if (createMatchButton != null)
        {
            if (lobbyPassword != null && lobbyName != null && isLobbyPublic != null)
            {
                createMatchButton.onClick.AddListener(() =>
                {
                    GameManager.instance.multiplayerManager.CreateLobby(lobbyName.text, !isLobbyPublic.isOn, lobbyPassword.text);
                });
            }
            else
            {
                Debug.LogError("One or more of the associated parameters for creating a lobby were null.");
            }
        }
        else
        {
            Debug.LogError("The createMatch button could not be found.");
        }
    }
}
