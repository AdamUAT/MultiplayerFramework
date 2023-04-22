using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingScreen : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.multiplayerManager.OnConnectingStarted += MultiplayerManager_OnConnectingStarted;

        gameObject.SetActive(false);
    }


    private void MultiplayerManager_OnConnectingStarted(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }
}
