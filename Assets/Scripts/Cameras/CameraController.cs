using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float cameraSpeed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 ScreenXLimits = Vector2.zero; 
    [SerializeField] private Vector2 ScreenZLimits = Vector2.zero;

    private Vector2 previousInput;

    private Controls controls;

    public override void OnStartAuthority()
    {
        // turn on the camera
        playerCameraTransform.gameObject.SetActive(true);

        // initliase the controls
        controls = new Controls();

        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;

        controls.Enable();

    }

    [ClientCallback]
    private void Update()
    {
        // stop camera cheats and also stop panning by accident
        if (!hasAuthority || !Application.isFocused) { return; }


        UpdateCameraPosition();
        
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();

    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;

        // if keyboard input
        if(previousInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            // vertical restrants
            if(cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;

            }
            else if (cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }

            // horizontal restrants
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;

            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            // move camera at a stable speed
            pos += cursorMovement.normalized * cameraSpeed * Time.deltaTime;

        }
        else
        {
            // move at a stable speed
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * cameraSpeed * Time.deltaTime;
        }

        // out of bounds check clamp vector 3 in vector 2 space
        pos.x = Mathf.Clamp(pos.x, ScreenXLimits.x, ScreenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, ScreenZLimits.x, ScreenZLimits.y);


        // move the camera
        playerCameraTransform.position = pos;



    }
}
