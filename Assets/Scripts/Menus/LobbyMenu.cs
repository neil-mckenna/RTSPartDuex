using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;

    [SerializeField] TMP_Text[] playerNameTexts = new TMP_Text[4];

    private void OnEnable()
    {
        // subscribe to our events
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;

    }

    private void OnDestroy()
    {
        // unsubscribe to our events
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
        RTSPlayer.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    private void HandleClientConnected()
    {
        Debug.Log("Handle client is was called");
        lobbyUI.SetActive(true);
    }

    private void HandleClientDisconnected()
    {
        
    }

    private void ClientHandleInfoUpdated()
    {
        List<RTSPlayer> players = ((RTSNetworkManager)NetworkManager.singleton).PlayersList;
        
        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].GetDisplayName();
        }

        for (int i = players.Count; i > playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting for Player...";
        }

        startGameButton.interactable = players.Count >= 2;
    }

    public void StartGame()
    {
        // get the rtsplayer component and call the the startgame command on the server

        Debug.LogWarning(NetworkClient.connection.identity + "" + NetworkClient.connection.identity.name);
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
