using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;

    // layer mask is a struct so needs a constructor rather than a null
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }


    private void Update()
    {
        // only right mouse input
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        // ray postion of mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // raycast to remove everything except the selection 
        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)){ return; }

        // if you select an enemy
        if(hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            // move if target my own units
            if (target.hasAuthority)
            {
                TryMove(hit.point);
                return;
            }

            // target enemy
            TryTarget(target);
            return;
        }

        // move just incase 
        TryMove(hit.point);

    }

    private void TryMove(Vector3 point)
    {
        // check the current selection list from the unitSelectionHandler
        // select the unit call teh unitMovementScript then Public Server Command Move to the hit.point

        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
            
        }
    }

    private void TryTarget(Targetable target)
    {

        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);

        }
    }


    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }



}
