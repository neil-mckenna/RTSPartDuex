using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField addressInput = null;
    [SerializeField] private Button joinButton = null;

    private void OnEnable()
    {
        // subscribe to our events
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;

    }

    private void OnDisable()
    {
        // unsubscribe to our events
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }



    public void Join()
    {
        Debug.Log("JoinlobbyMenu.Join() is called");

        // take the address from the input field 
        string address = addressInput.text;

        Debug.Log(address);

        // pass the address to the to network manager
        NetworkManager.singleton.networkAddress = address;

        // starts the client
        NetworkManager.singleton.StartClient();

        // disable join button
        joinButton.interactable = false;

    }

    // connecting to logic
    private void HandleClientConnected()
    {
        // renable the button
        joinButton.interactable = true;

        // turn off this object lobby menu
        gameObject.SetActive(false);

        // turn of landing page also
        landingPagePanel.SetActive(false);

    }

    // failed to connect logic
    private void HandleClientDisconnected()
    {
        // renable the button
        joinButton.interactable = true;


    }


}
