using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitBaseSpawnerPrefab = null;





    #region Server

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // an instance of red cube
        GameObject unitBaseSpawnerInstance = Instantiate(unitBaseSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

        // tell the server about spawning a base for other spawns with the player authority
        NetworkServer.Spawn(unitBaseSpawnerInstance, conn);

    }



    #endregion


    #region Client









    #endregion


}
