using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;


    private void OnEnable()
    {
        // subscribe to our events
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;

    }

    private void OnDestroy()
    {
        // unsubscribe to our events
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }

    private void HandleClientConnected()
    {
        Debug.Log("is was called");
        lobbyUI.SetActive(true);
    }

    private void HandleClientDisconnected()
    {
        
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



}
