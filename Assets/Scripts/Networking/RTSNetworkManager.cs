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

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        // grab hold a player object
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        // give the new player a random team color
        player.SetTeamColor(new Color(
            Random.Range(0.1f, 0.9f),
            Random.Range(0.1f, 0.9f),
            Random.Range(0.1f, 0.9f)
            ));

    }

    public override void OnStartServer()
    {
        base.OnStartServer();


    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // grab hold a player object
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        // give the new player a random team color
        player.SetTeamColor(new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
            ));

        // an instance of red cube
        GameObject unitBaseSpawnerInstance = Instantiate(unitBaseSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);

        Debug.Log(unitBaseSpawnerInstance);


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


    public override void OnStartClient()
    {
        Debug.Log("Client has started");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log("Connected to server " + conn + " id " + conn.connectionId + " id is " + conn.identity);
        ClientScene.AddPlayer(conn);

    }



    #endregion


}
