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
        rb.MovePosition(direction * speed * Time.deltaTime);
    }
}
