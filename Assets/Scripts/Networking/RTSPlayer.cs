using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Building[] buildings = new Building[0];

    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();


    // getter
    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }

    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }


    // Server and client Below

    #region Server

    // Server Events 
    public override void OnStartServer()
    {
        // listen for these events and pass them to the method
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
        
    }

    public override void OnStopServer()
    {
        // stop listening for these events
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    // Server Methods
    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {

        Building buildingToPlace = null;

        // search for the building then break out after 1 loop
        foreach (Building building in buildings)
        {
            if(building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        // null check
        if(buildingToPlace == null) { return; }

        // spawn an instance of this building
        GameObject tryPlaceBuildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);

        // pass instance to server and all clients with player authority
        NetworkServer.Spawn(tryPlaceBuildingInstance, connectionToClient);


    }



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

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Add(building);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        myBuildings.Remove(building);
    }

    #endregion
    //
    // space
    //
    #region Client

    // Client Events

    public override void OnStartAuthority()
    {
        // only client can call
        if (NetworkServer.active) { return; }

        // listen for these events and pass them to the method
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }  

    public override void OnStopClient()
    {
        // only client can call
        if (!isClientOnly || !hasAuthority) { return; }

        // listen for these events and pass them to the method
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }


    // Client Methods

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        // add the unit to my unit list
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {

        // remove the unit from unit list
        myUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }



    #endregion


}
