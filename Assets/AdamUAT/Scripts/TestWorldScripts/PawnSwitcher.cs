using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSwitcher : MonoBehaviour
{
    [SerializeField]
    private PawnPrefabsManager.Pawns newPawn;

    private void OnTriggerEnter(Collider other)
    {
        Pawn player = other.GetComponent<Pawn>();
        //Only will add the player if it already isn't inside of it.
        if(player != null)
        {
            if (player.Controller != null)
            {
                //Casts the controller to playerController.
                PlayerController playerController = player.Controller as PlayerController;

                //If the cast failed, then it is an AI and we ignore it.
                if (playerController != null)
                {
                    //The - before the + prevents duplicates.
                    playerController.Interact -= PawnSwitcher_Interact;
                    playerController.Interact += PawnSwitcher_Interact;
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
                    playerController.Interact -= PawnSwitcher_Interact;
                }
            }
            else
            {
                Debug.LogError("No controller connected to the trigger entered pawn.");
            }
        }
    }

    private void PawnSwitcher_Interact(object sender, System.EventArgs e)
    {

        //Casts the sender into the player reference.
        PlayerController player = sender as PlayerController;

        if(player != null)
        {
            player.SwitchPawnOnServerRpc(newPawn);
        }
    }
}