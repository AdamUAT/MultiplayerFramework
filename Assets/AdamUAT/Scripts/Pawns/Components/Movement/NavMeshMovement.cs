using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMovement : Movement
{
    NavMeshAgent navMeshAgent;

    protected void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if(navMeshAgent == null)
        {
            gameObject.AddComponent<NavMeshAgent>();
        }
    }

    public override void MoveTo(Vector3 position)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.destination = position;
    }

    public bool HasReachedDestination()
    {
        return (Mathf.Abs(navMeshAgent.remainingDistance) < 0.05);
    }

    public void Stop()
    {
        navMeshAgent.isStopped = true;
    }
}
