using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourceCost = 10;
    [SerializeField] private Health health = null;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;

    // Server Events
    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;
    // Clients Events
    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;


    // Getter to return UnitMovement to other Classes

    public int GetResourceCost()
    {
        return resourceCost;
    }

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }




    // Client and Server below

    #region Server

    // server events

    // tell the server about this unit by subscribing to an event
    public override void OnStartServer()
    {
        
        ServerOnUnitSpawned?.Invoke(this);

        health.ServerOnDie += ServerHandleDie;

    }

    // tell the server about this unit by subscribing to an event
    public override void OnStopServer()
    {
        
        ServerOnUnitDespawned?.Invoke(this);

        health.ServerOnDie -= ServerHandleDie;
    }


    // server methods

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }


    #endregion

    /// <summary>
    /// 
    /// </summary>

    #region Client

    // client events

    // for host and client
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        // if we are server or dont have authority do nothing
        if (!hasAuthority) { return; }

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    // client methods

    // client method to toggle select circle
    [Client]
    public void Select()
    {
        if (!hasAuthority) { return; }
        onSelected?.Invoke();
    }

    // client method to toggle deselect circle
    [Client]
    public void Deselect()
    {
        if (!hasAuthority) { return; }
        onDeselected?.Invoke();
    }



    #endregion


}
