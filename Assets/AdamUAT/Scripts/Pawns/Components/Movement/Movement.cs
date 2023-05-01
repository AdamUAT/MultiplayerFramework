using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : PawnComponent
{
    [SerializeField]
    [Tooltip("How fast this pawn moves, in units per second.")]
    protected float speed = 10.0f;

    /// <summary>
    /// Moves the pawn in a unit direction.
    /// </summary>
    public virtual void Move(Vector3 direction) { }
    public virtual void Move(Vector2 direction) { }

    public virtual void MoveTo(Vector3 position) { }

    public virtual void Rotate(Vector3 direction) { }
}
