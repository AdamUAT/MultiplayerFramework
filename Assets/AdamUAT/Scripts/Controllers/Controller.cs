using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Controller : NetworkBehaviour
{
    private Pawn pawn;
    public Pawn Pawn
    {
        get { return pawn; }
        set
        {
            pawn = value;

            //Since both Controller and Pawn set each other when they are set, it will cause an infinite loop if there isn't a check.
            if (pawn != null && pawn.Controller != this)
            {
                pawn.Controller = this;
            }
        }
    }
}
