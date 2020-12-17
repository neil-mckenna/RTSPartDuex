using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    private Camera mainCamera;

    NavMeshHit hit;
    NavMeshHit hitB;
    bool blocked = false;

    #region Server

    [Command]
    private void CmdMove(Vector3 position)
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

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        mainCamera = Camera.main;
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) {
            Debug.Log("No authority");
            return;
        };

        if (!Mouse.current.rightButton.wasPressedThisFrame){
            return; 
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) {

            Debug.Log("Didnt hit anything");
            return; 
        }

        CmdMove(hit.point);
    }

    #endregion
}
