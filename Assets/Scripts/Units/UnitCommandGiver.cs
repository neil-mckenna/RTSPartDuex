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
    }

    private void Update()
    {
        // only right mouse input
        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        // ray postion of mouse position
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // raycast to remove everything except the selection 
        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return;
        }

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
}
