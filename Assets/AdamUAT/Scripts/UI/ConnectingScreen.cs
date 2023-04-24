using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingScreen : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.multiplayerManager.OnConnectingStarted += ConnectingScreen_OnConnectingStarted;
        GameManager.instance.multiplayerManager.OnConnectingFinished += ConnectingScreen_OnConnectingFinished;

        gameObject.SetActive(false);
    }


    private void ConnectingScreen_OnConnectingStarted(object sender, System.EventArgs e)
    {
        gameObject.SetActive(true);
    }

    private void ConnectingScreen_OnConnectingFinished(object sender, System.EventArgs e)
    {
        gameObject.SetActive(false);
    }
}
