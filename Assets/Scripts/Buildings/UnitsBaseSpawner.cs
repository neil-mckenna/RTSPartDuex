using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UnitsBaseSpawner : NetworkBehaviour , IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }

    }


    #region Server

    // Server events

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }

    // Server methods

    [Server]
    private void ProduceUnits()
    {
        // performance nullcheck
        if(queuedUnits == 0) { return; }

        // starts timer
        unitTimer += Time.deltaTime;

        // check if timer is less than spawn duration
        if(unitTimer < unitSpawnDuration) { return; }

        // instance of a spawned prefab
        GameObject unitInstance = Instantiate(unitPrefab.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);

        // tell the server to spawn with client authority 
        NetworkServer.Spawn(unitInstance, connectionToClient);

        //  random number between 0,1 * by 7 for e.g. = the offset
        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        
        // since x and z and the vector movement keep the y height at 0 or same as the object
        spawnOffset.y = unitSpawnPoint.position.y;

        // grab the instance movement component
        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();

        // move to somewhere within our spawnoffset radius
        unitMovement.ServerMove(transform.position + spawnOffset);

        // reduce the units in the queue by 1
        queuedUnits--;

        // reset the timer
        unitTimer = 0f;

    }


    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        // cant queue any more that than max e.g. 5 for example
        if(queuedUnits == maxUnitQueue) { return; }

        // local player
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();

        // local int
        int resources = player.GetResources();

        // if you lack the resources then dont 
        if(resources < unitPrefab.GetResourceCost()) { return; }

        // add to teh queue
        queuedUnits++;

        // reduce the players resources
        player.SetResources(player.GetResources() - unitPrefab.GetResourceCost());


    }



    #endregion


    #region Client
    private void UpdateTimerDisplay()
    {
        // 0.1 to 1 for an easier conversion for the fill ratio
        float newProgress = unitTimer / unitSpawnDuration;

        // if progress is less than the fill max
        if(newProgress < unitProgressImage.fillAmount)
        {
            // reset on full circle
            unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(
                unitProgressImage.fillAmount,
                newProgress,
                ref progressImageVelocity,
                0.1f);
        }

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        // Only Left Clicks
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        // client has the authority
        if (!hasAuthority){ return; }

        // send Spawn unit command to server
        CmdSpawnUnit();
    }

    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        remainingUnitsText.text = newUnits.ToString();
    }



    #endregion



}
