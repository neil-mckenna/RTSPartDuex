using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitsBaseSpawner : NetworkBehaviour , IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;


    #region Server

    // server events

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }

    // server methods

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        // instance of a spawned prefab
        GameObject unitInstance = Instantiate(unitPrefab, unitSpawnPoint.position, unitSpawnPoint.rotation);

        // tell the server to spawn with client authority 
        NetworkServer.Spawn(unitInstance, connectionToClient);

    }



    #endregion


    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        // Only Left Clicks
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        // client has the authority
        if (!hasAuthority){ return; }

        // send Spawn unit command to server
        CmdSpawnUnit();
    }



    #endregion



}
