using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthogonalCharacterControllerMovement : CharacterControllerMovement
{ 
    /// <summary>
    /// This only moves the pawn orthagonolly, with forward taking priority.
    /// </summary>
    public override void Move(Vector2 direction)
    {
        if (direction.y != 0)
        {
            characterController.Move(Parent.transform.forward * direction.y * speed * Time.deltaTime);
        }
        else if (direction.x != 0)
        {
            characterController.Move(Parent.transform.right * direction.x * speed * Time.deltaTime);
        }
    }

    public override void Rotate(Vector3 direction)
    {
        //Do nothing, as this pawn should not rotate.
    }
}
