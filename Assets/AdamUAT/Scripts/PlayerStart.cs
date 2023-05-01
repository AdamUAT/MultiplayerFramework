using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerStart : MonoBehaviour
{
    [SerializeField]
    private PawnPrefabsManager.Pawns pawnToSpawn;

    void Start()
    {
        foreach (PlayerController player in GameManager.instance.players)
        {
            //only spawns a pawn for the local player.
            if (player.IsLocalPlayer)
            {
                player.SpawnPawnOnServerRpc(pawnToSpawn, transform.position, transform.rotation);
            }
        }
    }
}