using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnComponent : MonoBehaviour
{
    private Pawn parent;
    public Pawn Parent
    {
        get
        {
            return parent;
        }
        set
        {
            parent = value;
        }
    }
}
