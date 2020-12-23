using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;
    private RTSPlayer player;
   


    // Update is called once per frame
    void Update()
    {
        
        try
        {


            // remove this later dirty method to grab the player 
            if (player == null)
            {
                Debug.LogWarning("In ResourcesDisplay player is " + player);
                CallPlayer();

            }

            if (player != null)
            {
                ClientHandleResourcesUpdated(player.GetResources());

                player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
            }
        }

        catch (Exception e)
        {
            Debug.LogError("Errors: " + e.Message + e.StackTrace + e.InnerException);
        }
    
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
        
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources: {resources}";
    }

    public void CallPlayer()
    {
        // this is a workaround as player is not created until 2nd scene
        // null exception will still happen

        Debug.LogWarning("Network connection is " + NetworkConnection.LocalConnectionId);

        player = FindObjectOfType<RTSPlayer>();

        Debug.LogWarning("BuildingButton player is " + player);

    }


}
