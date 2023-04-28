using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectingScreen : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.multiplayerManager.OnDisconnectingStarted += DisconnectingScreen_OnConnectingStarted;
        GameManager.instance.multiplayerManager.OnDisconnectingFinished += DisconnectingScreen_OnConnectingFinished;

        gameObject.SetActive(false);
    }


    private void DisconnectingScreen_OnConnectingStarted(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void DisconnectingScreen_OnConnectingFinished(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameManager.instance.multiplayerManager.OnDisconnectingStarted -= DisconnectingScreen_OnConnectingStarted;
        GameManager.instance.multiplayerManager.OnDisconnectingFinished -= DisconnectingScreen_OnConnectingFinished;
    }
}
