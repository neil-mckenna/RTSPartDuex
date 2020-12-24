using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;


    private void OnEnable()
    {
        // subscribe to our events
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;

    }



    private void OnDestroy()
    {
        // unsubscribe to our events
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
    }

    private void HandleClientConnected()
    {
        Debug.Log("is was called");
        lobbyUI.SetActive(true);
    }

    private void HandleClientDisconnected()
    {
        
    }

    public void StartGame()
    {
        // get the rtsplayer component and call the the startgame command on the server
        NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
    }


    public void LeaveLobby()
    {
        // if your a host or a client
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            // kill  the host
            NetworkManager.singleton.StopHost();
        }
        else
        {
            // kill your a client
            NetworkManager.singleton.StopClient();

            // reload the start menu
            SceneManager.LoadScene(0);

        }

    }

    // this only shows start button when the player is ownerleader 
    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }



}
