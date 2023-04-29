using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerMovement : Movement
{
    protected CharacterController characterController;

    [SerializeField]
    [Tooltip("How fast this pawn moves, in units per second.")]
    protected float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if(characterController == null)
        {
            gameObject.AddComponent<CharacterController>();
            Debug.LogWarning("No character controller was found on pawn, so one was created.");

            //Initialize speed
            speed = 10.0f;
        }
    }

    public override void Move(Vector3 direction)
    {
        //converts the direction into a unit vector.
        Vector3.Normalize(direction);

        characterController.Move(direction * speed * Time.deltaTime);
    }
    public override void Move(Vector2 direction)
    {
        //converts the direction into a unit vector.
        Vector3.Normalize(direction);

        characterController.Move(direction * speed * Time.deltaTime);
    }
}
