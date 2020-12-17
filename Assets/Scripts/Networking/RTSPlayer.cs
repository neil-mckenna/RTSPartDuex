using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<Unit> myUnits = new List<Unit>();


    // getter
    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }


    // Server and client Below

    #region Server

    // Server Events 
    public override void OnStartServer()
    {
        // listen for these events and pass them to the method
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

        
    }

    public override void OnStopServer()
    {
        // stop listening for these events
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
    }

    // Server Methods 
    private void ServerHandleUnitSpawned(Unit unit)
    {
        // check to make sure the unit call belongs to the owner 
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        // add the unit to my unit list
        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        // check to make sure the unit call belongs to the owner 
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        // remove the unit from my unit list
        myUnits.Remove(unit);
    }

    #endregion
    //
    // space
    //
    #region Client

    // Client Events

    public override void OnStartClient()
    {
        // only client can call
        if (!isClientOnly) { return; }

        // listen for these events and pass them to the method
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    public override void OnStopClient()
    {
        // only client can call
        if (!isClientOnly) { return; }

        // listen for these events and pass them to the method
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }


    // Client Methods

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        // protection check
        if (!hasAuthority) { return; }

        // add the unit to my unit list
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        // protection check
        if (!hasAuthority) { return; }

        // remove the unit from unit list
        myUnits.Remove(unit);
    }


    #endregion


}
