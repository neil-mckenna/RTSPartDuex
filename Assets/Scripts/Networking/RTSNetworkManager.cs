using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitBaseSpawnerPrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;



    #region Server

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // an instance of red cube
        GameObject unitBaseSpawnerInstance = Instantiate(unitBaseSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

        // tell the server about spawning a base for other spawns with the player authority
        NetworkServer.Spawn(unitBaseSpawnerInstance, conn);

    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {

            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);


        }
    }



    #endregion


    #region Client









    #endregion


}
