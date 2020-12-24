using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private LayerMask buildingBlockLayerMask = new LayerMask();
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] float buildingRangeLimit = 200f;

    private Color teamColor = new Color();
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();

    [SyncVar(hook =(nameof(ClientHandleResourcesUpdated)))]
    private int resources = 500;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    

    public event Action<int> ClientOnResourcesUpdated;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;


    // Getters
    public string GetDisplayName()
    {
        return displayName;
    }


    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }



    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }



    public Color GetTeamColor()
    {
        return teamColor;
    }


    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }

    public List<Building> GetMyBuildings()
    {
        return myBuildings;
    }

    public int GetResources()
    {
        return resources;
    }


    // Server and client Below

    #region Server
    // Setters
    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }


    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }


    [Server]
    public void SetTeamColor(Color newTeamcolor)
    {
        teamColor = newTeamcolor;

    }



    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    // universal function

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {

        // this checks to see if we are overlapping with other buildings, if so return
        if (Physics.CheckBox(
            point + buildingCollider.center,
            buildingCollider.size / 2,
            Quaternion.identity,
            buildingBlockLayerMask
            ))
        {
            return false;
        }

        // range check
        foreach (Building building in myBuildings)
        {
            // Range check using square magnitude so have to square it(range * range) to get the correct value
            
            //Debug.LogWarning((point - building.transform.position).sqrMagnitude + " is <= to " + buildingRangeLimit * buildingRangeLimit);
            if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }




    // Server Events 
    public override void OnStartServer()
    {
        // listen for these events and pass them to the method
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;


        DontDestroyOnLoad(gameObject);
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
    public void CmdStartGame()
    {
        // not the owner cant start game
        if (!isPartyOwner) { return; }

        // start game with network manager casting
        ((RTSNetworkManager)NetworkManager.singleton).StartGame();

    }


    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Debug.Log("Building " + buildingId);

        Building buildingToPlace = null;

        // search for the building then break out after 1 loop
        foreach (Building building in buildings)
        {
            if(building.GetId() == buildingId)
            {
                buildingToPlace = building;
                if(buildingToPlace == null)
                {
                    Debug.Log("buildtoPlace is " + buildingToPlace);
                }
                
                break;
            }
        }

        // null check
        if(buildingToPlace == null) { return; }
        Debug.Log("buildtoPlace was " + buildingToPlace);

        // can player afford the building
        if (resources < buildingToPlace.GetPrice()) { return; }
        //Debug.Log("Resources " + resources + " unitPrice " + buildingToPlace.GetPrice());

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if(buildingCollider == null)
        {
            Debug.LogError(buildingCollider.gameObject + " missing");
        }

        Debug.LogWarning("Can place  building " + CanPlaceBuilding(buildingCollider, point));
        Debug.LogWarning(buildingCollider + " " + point);
        if(!CanPlaceBuilding(buildingCollider, point)) { return; }

        // spawn an instance of this building
        GameObject tryPlaceBuildingInstance = Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);

        // pass instance to server and all clients with player authority
        NetworkServer.Spawn(tryPlaceBuildingInstance, connectionToClient);

        // update the resources - the cost
        SetResources(resources - buildingToPlace.GetPrice());
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

    public override void OnStartClient()
    {
        // only client can call
        if (NetworkServer.active) { return; }

        DontDestroyOnLoad(gameObject);

        // cast as server as an override and pass in the player to be added to list
        ((RTSNetworkManager)NetworkManager.singleton).PlayersList.Add(this);

    }


    public override void OnStopClient()
    {
        // 
        ClientOnInfoUpdated?.Invoke();

        // stop at the server
        if (!isClientOnly) { return; }

        // cast as server as an override and pass in the player to be added to list
        ((RTSNetworkManager)NetworkManager.singleton).PlayersList.Remove(this);

        // stop if unauthorised client
        if (!hasAuthority) { return; }

        // listen for these events and pass them to the method
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }


    // Client Methods

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);


    }


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
