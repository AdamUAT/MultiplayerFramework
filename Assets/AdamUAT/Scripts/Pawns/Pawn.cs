using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pawn : NetworkBehaviour
{
    private Controller controller;
    public Controller Controller 
    { 
        get 
        {
            return controller;
        } 
        set 
        {
            controller = value;

            //Since both Controller and Pawn set each other when they are set, it will cause an infinite loop if there isn't a check.
            if(controller != null && controller.Pawn != this)
            {
                controller.Pawn = this;
            }
        } 
    }

    private Movement movement;
    public Movement Movement
    {
        get
        {
            return movement;
        }
        set
        {
            movement = value;

            //Since both Controller and Pawn set each other when they are set, it will cause an infinite loop if there isn't a check.
            if (movement != null && movement.Parent != this)
            {
                movement.Parent = this;
            }
        }
    }

    [ClientRpc]
    public void AssignReferencesClientRpc(NetworkObjectReference playerControllerReference)
    {

        //Assign the movement component
        Movement = GetComponent<Movement>();
        if (Movement == null)
        {
            Movement = gameObject.AddComponent<DefaultMovement>();
        }

        playerControllerReference.TryGet(out NetworkObject playerController);
        Controller = playerController.GetComponent<Controller>();
    }
}
