using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HostDisconnected : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += HostDisconnected_OnClientDisconnectCallback;

        //Starts invisible.
        gameObject.SetActive(false);
    }

    private void HostDisconnected_OnClientDisconnectCallback(ulong clientID)
    {
        //Checks if the server was the one who disconnected
        if(clientID == NetworkManager.ServerClientId)
        {
            gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
