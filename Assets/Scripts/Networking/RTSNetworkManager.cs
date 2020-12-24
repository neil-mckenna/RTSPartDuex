using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitBaseSpawnerPrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

    // listener events for UI 
    public static event Action ClientOnConnected; 
    public static event Action ClientOnDisconnected; 


    #region Server

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // grab hold a player object
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        // give the new player a random team color
        player.SetTeamColor(new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
            ));

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
        base.OnClientConnect(conn);

        // raise our events
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        // raise our events
        ClientOnDisconnected?.Invoke();
    }



    #endregion


}
