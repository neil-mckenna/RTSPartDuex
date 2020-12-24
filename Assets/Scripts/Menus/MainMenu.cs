using Mirror;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;

    [SerializeField] private bool useSteam = false;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;


    private void Start()
    {
        // if we dont want use steam just return
        if (useSteam) { return; }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

    }

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        if (useSteam)
        {
            // 4 steam friends only
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            return;
        }

        // start hosting
        NetworkManager.singleton.StartHost();
        
        //debug to see if is active
        Debug.LogWarning("Hostinglobby?" + NetworkManager.singleton.isActiveAndEnabled);

    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            // it failed to create a steam lobby
            landingPagePanel.SetActive(true);
            return;
        }
        // start
        NetworkManager.singleton.StartHost();

        // connect by steam id with loccy callback data and my steam user id
        SteamMatchmaking.SetLobbyData(new CSteamID(
            callback.m_ulSteamIDLobby),
            "HostAddress",
            SteamUser.GetSteamID().ToString());

        Debug.LogWarning(SteamUser.GetSteamID().ToString());
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        // join a steam lobby
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        // dont do anything if I am the host
        if (NetworkServer.active) { return; }

        // get the host address 
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            "HostAddress");

        // add ip address to newwork
        NetworkManager.singleton.networkAddress = hostAddress;

        // start 
        NetworkManager.singleton.StartClient();

        // turn off landing page
        landingPagePanel.SetActive(false);


    }

}
