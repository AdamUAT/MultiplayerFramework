using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : Controller
{
    protected float timeOfLastStateChange;

    protected override void Start()
    {
        base.Start();

        if (IsServer)
        {
            timeOfLastStateChange = Time.time;
        }
    }

    void Update()
    {
        if (IsServer)
        {
            DoFSM();
        }
    }

    protected virtual void DoFSM()
    {

    }

    protected virtual void ChangeStateTime()
    {
        timeOfLastStateChange = Time.time;
    }
}
