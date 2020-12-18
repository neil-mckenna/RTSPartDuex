using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    // server Actions
    public static event Action ServerOnGameOver;

    // client Actions
    public static event Action<string> ClientOnGameOver; 

    private List<UnitBase> bases = new List<UnitBase>();


    #region Server

    // Server Events

    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    // Server methods
    [Server]
    public void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
        
    }

    [Server]
    public void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        if(bases.Count != 1) { return; }

        //Debug.LogWarning("Game is over");

        // grab the id of the last remaining player
        int playerId = bases[0].connectionToClient.connectionId;

        // pass id to the method
        RpcGameOver($"Player {playerId}");

        ServerOnGameOver?.Invoke();
    }

    #endregion

    ///

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
