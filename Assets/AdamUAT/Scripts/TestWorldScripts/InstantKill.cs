using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantKill : MonoBehaviour
{
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
                    playerController.Interact -= InstantKill_Interact;
                    playerController.Interact += InstantKill_Interact;
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
                    playerController.Interact -= InstantKill_Interact;
                }
            }
            else
            {
                Debug.LogError("No controller connected to the trigger entered pawn.");
            }
        }
    }

    private void InstantKill_Interact(object sender, System.EventArgs e)
    {
        PlayerController player = sender as PlayerController;

        if (player != null)
        {
            if (player.Health != null)
            {
                player.Health.TakeDamage(player.Health.CurrentHealth);
            }
            else
            {
                Debug.LogError("DamageSource could not get Health component from controller.");
            }
        }
    }
}
