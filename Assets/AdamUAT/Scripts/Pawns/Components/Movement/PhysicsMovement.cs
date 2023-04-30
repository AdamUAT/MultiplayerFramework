using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsMovement : Movement
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// This only moves the pawn orthagonolly, with forward taking priority.
    /// </summary>
    public override void Move(Vector2 direction)
    {
        Vector3 direction3 = new Vector3(direction.x, 0, direction.y);
        rb.MovePosition(Parent.transform.position + direction3 * speed * Time.deltaTime);
    }
}
