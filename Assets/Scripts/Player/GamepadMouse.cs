using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class GamepadMouse : MonoBehaviour
{
    [SerializeField] private PlayerInput playerControls;
    [SerializeField] private RectTransform cursorPos;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private RectTransform canvasTranform;
    private Canvas myCanvas;
    private bool previousMouseState;
    private Camera mainCamera;
    private Mouse virtualMouse;
    private void OnEnable()
    {
        mainCamera = Camera.main;
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        } else if (virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse);
        if (cursorPos != null)
        {
            Vector2 position = cursorPos.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }
        InputSystem.onAfterUpdate += UpdateMotion;
    }

    private void OnDisable()
    {
        InputSystem.onAfterUpdate -= UpdateMotion;
    }
    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
            
        }

        Vector2 deltaValue = Gamepad.current.rightStick.ReadValue();
        deltaValue *= moveSpeed * Time.deltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;
        
        newPosition.x = Mathf.Clamp(newPosition.x, 0f, Screen.width);
        newPosition.y = Mathf.Clamp(newPosition.y, 0f, Screen.height);
        
        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        bool abuttonIsPressed = Gamepad.current.aButton.isPressed;
        if (previousMouseState != abuttonIsPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, abuttonIsPressed);
            InputState.Change(virtualMouse,mouseState);
            previousMouseState = abuttonIsPressed;
        }

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTranform, position,  myCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera, out anchoredPosition);
        cursorPos.anchoredPosition = anchoredPosition;
    }
}
