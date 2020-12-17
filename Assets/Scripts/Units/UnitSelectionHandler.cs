using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask(); 
    private Camera mainCamera;

    private List<Unit> selectedUnits = new List<Unit>();


    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }

            // clear the list
            selectedUnits.Clear();

        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }

    }

    private void ClearSelectionArea()
    {
        // ray from mouse point
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        // return what the raycast hit regading the layermask
        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            return;
        }

        // test if the ray hits a unit  
        if(!hit.collider.TryGetComponent(out Unit unit)) { return; }

        // remove a selection that does not belong to us
        if (!unit.hasAuthority) { return; }

        selectedUnits.Add(unit);

        // Select all units in the list  that have a collider to have a green selection circle toggled
        foreach (Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Select();
        }
    }
}
