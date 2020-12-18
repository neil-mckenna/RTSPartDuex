using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private Sprite iconSprite = null;
    [SerializeField] private int id = -1;
    [SerializeField] private int price = 100;


    // Server Events
    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    // Client Events
    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;


    public Sprite GetIcon()
    {
        return iconSprite;
    }

    public int GetId()
    {
        return id;
    }

    public int GetPrice()
    {
        return price;
    }

    #region Server

    // Server event calls
    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }

    // server methods

    #endregion
    ///
    #region Client

    // Client Event calls

    public override void OnStartClient()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        AuthorityOnBuildingDespawned?.Invoke(this);
    }

    // Client Methods



    #endregion


}
