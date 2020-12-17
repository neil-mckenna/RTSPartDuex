using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;

    NavMeshHit hit;
    NavMeshHit hitB;
    bool blocked = false;



    // Server and Client

    #region Server

    [Command]
    public void CmdMove(Vector3 position)
    {

       //Debug.Log(agent.isOnNavMesh);
       if(!NavMesh.SamplePosition(position, out hit, 0.5f, NavMesh.AllAreas)) {
            return;
        }
        agent.SetDestination(hit.position);
    }

    public void FixedUpdate()
    {
        blocked = NavMesh.Raycast(transform.position, hit.position, out hitB, NavMesh.AllAreas);
        Debug.DrawLine(transform.position, hit.position, blocked ? Color.red : Color.green);
        if (blocked) Debug.DrawLine(hitB.position, Vector3.up, Color.red);
    }

    #endregion

    /// <summary>
    /// split between server and client
    /// </summary>

    #region Client

    #endregion
}
