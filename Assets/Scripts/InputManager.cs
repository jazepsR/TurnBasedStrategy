#define USE_NEW_INPUT_SYSTEM

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    private PlayerInputActions playerInputActions;
    private void Awake()
    {
        instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }

    public bool IsMouseButtonDown()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.Click.WasPressedThisFrame();
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    public Vector2 GetCameraMoveVector()
    {
#if USE_NEW_INPUT_SYSTEM
        if (playerInputActions.Player.Click.IsPressed())
        {
            return playerInputActions.Player.MousePosition.ReadValue<Vector2>().normalized;
        }
        else
        {
            return playerInputActions.Player.CameraMovement.ReadValue<Vector2>();
        }
#else
        Vector2 inputMoveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            inputMoveDirection.y += 1f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            inputMoveDirection.y -= 1f;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            inputMoveDirection.x -= 1f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            inputMoveDirection.x += 1f;
        }
        return inputMoveDirection;
        
#endif
    }

    public float GetRotationValue()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraRotate.ReadValue<float>();
#else
        float rotationValue = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            rotationValue += 1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotationValue -= 1f;
        }
        return rotationValue;        
#endif
    }

    public float GetZoomValue()
    {
#if USE_NEW_INPUT_SYSTEM
        return playerInputActions.Player.CameraZoom.ReadValue<float>();
#else
        return Input.mouseScrollDelta.y;
#endif
    }
}
