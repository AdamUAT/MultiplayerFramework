using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class FriendlySpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject friendlyAIControllerPrefab;
    [SerializeField]
    private PawnPrefabsManager.Pawns pawnToSpawn;
    [SerializeField]
    private BoxCollider wanderArea;

    private void OnTriggerEnter(Collider other)
    {
        Pawn player = other.GetComponent<Pawn>();
        //Only will add the player if it already isn't inside of it.
        if (player != null)
        {
            if (player.Controller != null)
            {
                //Casts the controller to playerController.
                PlayerController playerController = player.Controller as PlayerController;

                //If it is not a playerController, then we ignore it since it is an AI.
                if (playerController != null)
                {
                    playerController.Interact -= FriendlySpawner_Interact;
                    playerController.Interact += FriendlySpawner_Interact;
                }
            }
            else
            {
                Debug.LogError("No controller connected to the trigger entered pawn.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Pawn player = other.GetComponent<Pawn>();
        //Only will add the player if it already isn't inside of it.
        if (player != null)
        {
            if (player.Controller != null)
            {
                //Casts the controller to playerController.
                PlayerController playerController = player.Controller as PlayerController;

                //If the cast failed, then it is an AI and we ignore it.
                if (playerController != null)
                {
                    //Remove the this from the interact button.
                    playerController.Interact -= FriendlySpawner_Interact;
                }
            }
            else
            {
                Debug.LogError("No controller connected to the trigger entered pawn.");
            }
        }
    }

    private void FriendlySpawner_Interact(object sender, System.EventArgs e)
    {
        SpawnFriendlyOnServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnFriendlyOnServerRpc()
    {
        //Spawn the controller.
        GameObject botGameObject = Instantiate(friendlyAIControllerPrefab);
        FriendlyAIController bot = botGameObject.GetComponent<FriendlyAIController>();
        bot.wanderArea = wanderArea;

        //Random position for the friendly to spawn.
        Vector3 spawnPosition = wanderArea.bounds.center + new Vector3(
           (UnityEngine.Random.value - 0.5f) * wanderArea.bounds.size.x,
           (UnityEngine.Random.value - 0.5f) * wanderArea.bounds.size.y,
           (UnityEngine.Random.value - 0.5f) * wanderArea.bounds.size.z
        );

        Pawn pawn = Instantiate(GameManager.instance.pawnPrefabsManager.pawnPrefabsDictionary.GetValueOrDefault(pawnToSpawn), spawnPosition, Quaternion.identity).GetComponent<Pawn>();

        bot.Pawn = pawn;

        //Spawn the pawn on the server
        NetworkObject pawnNetworkObject = pawn.GetComponent<NetworkObject>();
        if (pawnNetworkObject != null)
        {
            pawnNetworkObject.Spawn();
        }
        else
        {
            Debug.LogError("No NetworkObject found on pawn. It has been desynchronized from the network.");
        }

        //Spawn the controller on the server.
        NetworkObject controllerNetworkObject = bot.GetComponent<NetworkObject>();
        if (controllerNetworkObject != null)
        {
            controllerNetworkObject.Spawn();
        }
        else
        {
            Debug.LogError("No NetworkObject found on pawn. It has been desynchronized from the network.");
        }

        pawn.AssignReferencesClientRpc(controllerNetworkObject);
    }
}
