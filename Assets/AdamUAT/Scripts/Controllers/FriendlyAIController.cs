using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyAIController : AIController
{
    [HideInInspector]
    public BoxCollider wanderArea;
    [SerializeField]
    private AudioClip wanderOverNoise;

    private enum FriendlyState { Wander, Wait}
    private FriendlyState currentState;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        currentState = FriendlyState.Wait;
    }

    protected override void DoFSM()
    {
        switch(currentState)
        {
            case FriendlyState.Wait:
                //Wait 5 seconds
                if(timeOfLastStateChange + 2 < Time.time)
                {
                    Vector3 targetLocation = wanderArea.bounds.center + new Vector3(
   (UnityEngine.Random.value - 0.5f) * wanderArea.bounds.size.x,
   (UnityEngine.Random.value - 0.5f) * wanderArea.bounds.size.y,
   (UnityEngine.Random.value - 0.5f) * wanderArea.bounds.size.z
);
                    Pawn.Movement.MoveTo(targetLocation);

                    currentState = FriendlyState.Wander;
                    ChangeStateTime();
                }
                break;
            case FriendlyState.Wander:
                NavMeshMovement navMeshMovement = Pawn.Movement as NavMeshMovement;
                if(navMeshMovement != null)
                {
                    if (navMeshMovement.HasReachedDestination() || timeOfLastStateChange + 15 < Time.time)
                    {
                        navMeshMovement.Stop();

                        AudioSource audioSource = GetComponent<AudioSource>();
                        if(audioSource != null)
                        {
                            audioSource.Play();
                        }
                        else
                        {
                            Debug.LogError("No audioSource component on FriendlyAIController.");
                        }

                        currentState = FriendlyState.Wait;
                        ChangeStateTime();
                    }
                }
                else
                {
                    Debug.Log("The movement component on this FriendlyAIController is not a NavMeshMovement!");
                }

                break;
            default:
                Debug.LogError("FriendlyAIController FSM state went out of bounds.");
                break;
        }

    }
}
