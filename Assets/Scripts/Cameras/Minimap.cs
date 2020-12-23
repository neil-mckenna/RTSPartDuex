using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform miniMapRect = null;
    // assuming mapscale is in a square
    [SerializeField] private float mapScale = 50f;

    [SerializeField] private float offset = -6f;

    private Transform playerCameraTransform;
    private RTSPlayer player;

    private void Update()
    {
        if(playerCameraTransform != null) { return; }

        if(NetworkClient.connection.identity == null)
        {
            return;
        }

        if(player == null){
            CallPlayer();
        }

        playerCameraTransform = player.GetCameraTransform();
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }



    private void MoveCamera()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // method to check if click inside the canvas rectangle and return vector2 values
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            miniMapRect,
            mousePos,
            null,
            out Vector2 localPoint
            )){ return; }

        // convert the vector coords in to a percentage of height and width into the screen position
        Vector2 lerp = new Vector2(
            (localPoint.x - miniMapRect.rect.x) / miniMapRect.rect.width,
            (localPoint.y - miniMapRect.rect.y) / miniMapRect.rect.height
            );

        // x is x , y is 0 same hieght, but z it 2d vector for y
        // assuming maps is square
        Vector3 newCameraPos = new Vector3(
            Mathf.Lerp(-mapScale, mapScale, lerp.x),
            playerCameraTransform.position.y,
            Mathf.Lerp(-mapScale, mapScale, lerp.y)
            );

        // camera to new position with an offset on z
        playerCameraTransform.position = newCameraPos + new Vector3(0,0,offset);


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
