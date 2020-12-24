using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private TMP_Text priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    private Camera mainCamera;
    private BoxCollider buildingCollider;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;


    private void Start()
    {
        mainCamera = Camera.main;

        iconImage.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();

        buildingCollider = building.GetComponent<BoxCollider>();

    }

    private void Update()
    {
        // remove this later dirty method to grab the player 
        if (player == null)
        {
            
            Invoke("CallPlayer", Mathf.Epsilon);
        }


        if (buildingPreviewInstance == null) { return; }

        if (buildingCollider == null)
        {
            Debug.LogWarning("No collider " + buildingCollider.gameObject);
        }

        UpdateBuildingPreview();


    }

    public void OnPointerDown(PointerEventData eventData)
    {


        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        if (player == null)
        {
            Debug.LogError("Player in pointer " + player.gameObject);
        }

        if (building == null)
        {
            Debug.LogError("Building in pointer " + building);
        }

        // stop if price is too much
        if (player.GetResources() < building.GetPrice()) { return; }


        buildingPreviewInstance = Instantiate(building.GetPreview());

        Debug.LogWarning("preview "+ buildingPreviewInstance.gameObject);

        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(buildingPreviewInstance == null) {
            Debug.LogError("No buildpreview gameobject passed: " + eventData.selectedObject);  
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {

            player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }

        Destroy(buildingPreviewInstance);

    }

    public void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }

        buildingPreviewInstance.transform.position = hit.point;


        if (!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }

        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;

        buildingRendererInstance.material.SetColor("_BaseColor", color);

    }

    public void CallPlayer()
    {
        // this is a workaround as player is not created until 2nd scene
        // null exception will still happen

        Debug.LogWarning("Network connection id is " + NetworkConnection.LocalConnectionId);

        player = FindObjectOfType<RTSPlayer>();
        
        //Debug.LogWarning("BuildingButton player is " + player.gameObject);

    }



}
