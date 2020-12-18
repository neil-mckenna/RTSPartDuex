using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] private RectTransform unitSelectionDragBox = null;

    [SerializeField] private LayerMask layerMask = new LayerMask();


    // state
    private Vector2 startPosition;

    private RTSPlayer player;
    private Camera mainCamera;
    public List<Unit> SelectedUnits { get; } = new List<Unit>();


    private void Start()
    {
        mainCamera = Camera.main;

        // subscribe to events

        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

    }

    private void OnDestroy()
    {
        // unsubscribe to events
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    private void Update()
    {
        // remove this later dirty method to grab the player 
        if (player == null)
        {
            Invoke("CallPlayer",1f);
        }
    

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();

        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }

    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }

            // clear the list
            SelectedUnits.Clear();
        }


        // turn on dragbox
        unitSelectionDragBox.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    private void UpdateSelectionArea()
    {
        // mouse positon with x,y values
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // store the difference of value between these 2 points for x and y
        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        // change the size of the box via the absoulute value of with and height
        unitSelectionDragBox.sizeDelta = new Vector2(
            Mathf.Abs(areaWidth),
            Mathf.Abs(areaHeight));

        // change the anchor to middle fo these 2 values
        unitSelectionDragBox.anchoredPosition = startPosition + new Vector2(
            areaWidth / 2,
            areaHeight / 2
            );

    }

    private void ClearSelectionArea()
    {
        // turn off DragBox
        unitSelectionDragBox.gameObject.SetActive(false);

        // single click
        if(unitSelectionDragBox.sizeDelta.magnitude == 0)
        {
            // ray from mouse point
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            // return what the raycast hit regading the layermask
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                return;
            }

            // test if the ray hits a unit  
            if (!hit.collider.TryGetComponent(out Unit unit)) { return; }

            // remove a selection that does not belong to us
            if (!unit.hasAuthority) { return; }

            SelectedUnits.Add(unit);

            // Select all units in the list  that have a collider to have a green selection circle toggled
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();
            }

            return;
        }

        Vector2 min = unitSelectionDragBox.anchoredPosition - (unitSelectionDragBox.sizeDelta / 2);
        Vector2 max = unitSelectionDragBox.anchoredPosition + (unitSelectionDragBox.sizeDelta / 2);

        // target all units inside my own units
        foreach (Unit unit in player.GetMyUnits())
        {
            // if already selected continue onwards
            if (SelectedUnits.Contains(unit)) { continue; }

            // Convert 3d to 2d world
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            // check to see if units are inside the box
            if(screenPosition.x > min.x &&
                screenPosition.x < max.x &&
                screenPosition.y > min.y &&
                screenPosition.y < max.y)
            {
                // add to selection list
                SelectedUnits.Add(unit);

                // show the select green circle
                unit.Select();

            }

        }
   
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        // this remove dead units from the selection handlers and stops null refs
        SelectedUnits.Remove(unit);
    }



    public void CallPlayer()
    {
        // this is a workaround as player is not created until 2nd scene
        // null exception will still happen
        
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        Debug.LogWarning(player);
        Debug.LogWarning(NetworkClient.connection.identity.GetComponent<RTSPlayer>());
        //

    }


}
