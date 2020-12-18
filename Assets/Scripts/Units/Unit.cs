using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
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

    public override void OnStartServer()
    {
        // tell the server about this unit by subscribing to an event
        
        ServerOnUnitSpawned?.Invoke(this);

    }

    public override void OnStopServer()
    {
        // tell the server about this unit by subscribing to an event
        
        ServerOnUnitDespawned?.Invoke(this);
    }


    #endregion

    #region Client

    public override void OnStartClient()
    {
        // if we are server or dont have authority do nothing
        if (!isClientOnly || !hasAuthority) { return; }

        AuthorityOnUnitSpawned?.Invoke(this);
    }


    public override void OnStopClient()
    {
        // if we are server or dont have authority do nothing
        if (!isClientOnly || !hasAuthority) { return; }

        AuthorityOnUnitDespawned?.Invoke(this);
    }



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
