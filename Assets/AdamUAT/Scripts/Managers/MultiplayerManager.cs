using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using Unity.Services.Core;

public class MultiplayerManager : MonoBehaviour
{
    //public UnityTransport unityTransport { get; private set; }


    //Do all the one-time code relay stuff that allows it to work
    //multiplayerManager.PrimeRelay();

    public async void PrimeRelay()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    /// <summary>
    /// The current protocol of the transport is not set to Relay by default, so this will just run a local host and does not need internet connection.
    /// </summary>
    public void StartSinglePlayer()
    {
        NetworkManager.Singleton.StartHost();
    }

    /*
try
{
    //CreateAllocationAsync's parameter is the number of players, not including the host.
    Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

    RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
    //RelayServerData relayServerData1 = new RelayServerData();

    NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

    NetworkManager.Singleton.StartHost();
}
catch(RelayServiceException exception)
{
    Debug.LogException(exception);
}
*/
}
