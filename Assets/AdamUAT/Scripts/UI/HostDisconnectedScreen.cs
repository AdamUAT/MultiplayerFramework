using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostDisconnectedScreen : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.multiplayerManager.OnHostDisconnected += HostDisconnectedScreen_OnHostDisconnected;

        gameObject.SetActive(false);
    }

    private void HostDisconnectedScreen_OnHostDisconnected(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// The user presses a button to return to the multiplayer menu.
    /// </summary>
    public void Acknowledge()
    {
        gameObject.SetActive(false);
        GameManager.instance.gameStateManager.ChangeGameState(GameStateManager.GameState.HostOrJoin);
    }
}
