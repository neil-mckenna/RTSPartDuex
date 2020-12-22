using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] float chaseRange = 10f;


    NavMeshHit hit;

    // Server and Client

    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }


    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();

        if(targeter.GetTarget() != null)
        {
            // fancy square root to measure if target is inside the chase range
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                // stop basically
                agent.ResetPath();
            }


            return;
        }



        // allows them to return to there path
        if (!agent.hasPath) { return; }

        // to stop units fighting over path position
        if(agent.remainingDistance > agent.stoppingDistance) { return; }

        //
        agent.ResetPath();
    }

    
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    [Server]
    public void ServerMove(Vector3 position)
    {
        // moving clears target
        targeter.ClearTarget();

        //Debug.Log(agent.isOnNavMesh);
        if (!NavMesh.SamplePosition(position, out hit, 0.5f, NavMesh.AllAreas))
        {
            return;
        }
        agent.SetDestination(hit.position);
    }




    #endregion

    /// <summary>
    /// split between server and client
    /// </summary>

    #region Client

    [Command]
    public void CmdMove(Vector3 position)
    {

        ServerMove(position);
    }

    #endregion
}
