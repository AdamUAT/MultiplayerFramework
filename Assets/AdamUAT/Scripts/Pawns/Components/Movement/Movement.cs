using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : PawnComponent
{
    /// <summary>
    /// Moves the pawn in a unit direction.
    /// </summary>
    public virtual void Move(Vector3 direction) { }
    public virtual void Move(Vector2 direction) { }
}
