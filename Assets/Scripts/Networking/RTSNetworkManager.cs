using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject playerStartingBasePrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

    // listener events for UI 
    public static event Action ClientOnConnected; 
    public static event Action ClientOnDisconnected; 

    private bool isGameInProgress = false;

    public List<RTSPlayer> PlayersList { get; } = new List<RTSPlayer>();

    private const string  FIRST_MAP = "Scene_Map_01";  

    #region Server

    public override void OnServerConnect(NetworkConnection conn)
    {
        // if game is not in progress
        Debug.LogWarning("Game is in Progress " + isGameInProgress);
        if (!isGameInProgress) { return; }

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // grab the player that disconneded
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        // remove the player from the players list
        PlayersList.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        // clear all the players list on server stop
        PlayersList.Clear();

        isGameInProgress = false;
   
    }

    public void StartGame()
    {
        // start game with minimum of 2 players
        if(PlayersList.Count < 2){
            Debug.LogError("Not Enough players!, player count is : " + PlayersList.Count);
            return; }

        // stop people from joining midgame
        isGameInProgress = true;

        // change to first level map scene
        ServerChangeScene(FIRST_MAP);

    }


    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // grab hold a player object
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        // add player to playerlist
        PlayersList.Add(player);

        player.SetDisplayName($"Player {PlayersList.Count}");

        // give the new player a random team color
        player.SetTeamColor(new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
            ));

        // set the party owner to his first in lobby
        player.SetPartyOwner(PlayersList.Count == 1);


    }


    public override void OnServerSceneChanged(string sceneName)
    {

        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            // add a gameoverhandler to scene
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            // tell the server and clients about this
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            // loop for all the players and spawn a base
            foreach (RTSPlayer player in PlayersList)
            {
                Debug.LogWarning(player.name);
                // Create an object and spawn the baseprefab, at Mirror inbuilt start positions with normal rotation
                GameObject baseInstance = Instantiate(playerStartingBasePrefab, GetStartPosition().position, Quaternion.identity);

                // tell the server and clients about this passing object and its authority
                NetworkServer.Spawn(baseInstance, player.connectionToClient);


            }

        }
    }



    #endregion


    #region Client


    public override void OnStartClient()
    {
        Debug.Log("Client has started");
    }

    public override void OnStopClient()
    {
        // clear the players list on stop
        PlayersList.Clear();
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
