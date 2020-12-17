using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;

    NavMeshHit hit;

    // Server and Client

    #region Server

    [ServerCallback]
    private void Update()
    {
        // allows them to return to there path
        if (!agent.hasPath) { return; }

        // to stop units fighting over path position
        if(agent.remainingDistance > agent.stoppingDistance) { return; }
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {

       //Debug.Log(agent.isOnNavMesh);
       if(!NavMesh.SamplePosition(position, out hit, 0.5f, NavMesh.AllAreas)) {
            return;
        }
        agent.SetDestination(hit.position);
    }

    #endregion

    /// <summary>
    /// split between server and client
    /// </summary>

    #region Client

    #endregion
}
