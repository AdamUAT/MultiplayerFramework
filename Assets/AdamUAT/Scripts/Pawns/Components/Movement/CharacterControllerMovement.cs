using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMovement : Movement
{
    protected CharacterController characterController;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public override void Move(Vector3 direction)
    {
        //converts the direction into a unit vector.
        Vector3.Normalize(direction);

        direction = Parent.transform.InverseTransformDirection(direction);

        characterController.Move(direction * speed * Time.deltaTime);
    }
    public override void Move(Vector2 direction)
    {
        Vector3 direction3 = new Vector3(direction.x, 0, direction.y);

        //converts the direction into a unit vector.
        //Vector3.Normalize(direction3);

        direction3 = Parent.transform.TransformDirection(direction3);

        characterController.Move(direction3 * speed * Time.deltaTime);
    }

    public override void Rotate(Vector3 direction)
    {
        Parent.transform.Rotate(direction);
    }
}
