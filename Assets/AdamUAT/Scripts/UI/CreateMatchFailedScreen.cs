using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMatchFailedScreen : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.multiplayerManager.OnCreateLobbyFailed += CreateMatchFailedScreen_OnCreateLobbyFailed;

        gameObject.SetActive(false);
    }

    private void CreateMatchFailedScreen_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// The user presses a button to return to the multiplayer menu.
    /// </summary>
    public void Acknowledge()
    {
        gameObject.SetActive(false);
    }
}
