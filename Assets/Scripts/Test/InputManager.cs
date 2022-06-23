using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get { return _instance; }
    }
    private CharacterControls _characterControls;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        _characterControls = new CharacterControls();
        Cursor.visible = false;
    }

    
    private void OnEnable()
    {
        _characterControls.Enable();
        
        _characterControls.Land.Jump.performed += DoJump;
        _characterControls.Land.Jump.Enable();

        _characterControls.Land.Zoom.performed += IsZooming;
        _characterControls.Land.Zoom.Enable();
    }

    private void DoJump (InputAction.CallbackContext obj)
    {
        Debug.Log("Jump");
    }

    private void OnDisable()
    {
        _characterControls.Disable();
        _characterControls.Land.Jump.performed -= DoJump;
        _characterControls.Land.Jump.Disable();
        
        _characterControls.Land.Zoom.performed -= IsZooming;
        _characterControls.Land.Zoom.Disable();
    }
    
    

    public Vector2 GetPlayerMovement()
    {
        return _characterControls.Land.Move.ReadValue<Vector2>();
    }
    
    public Vector2 GetMouseDelta()
    {
        return _characterControls.Land.Look.ReadValue<Vector2>();
    }
    
    public bool HasJumpedThisFrame()
    {
        return _characterControls.Land.Jump.triggered;
    }

    public bool IsSprinting()
    {
        return _characterControls.Land.Sprint.triggered;
    }

    public void IsZooming(InputAction.CallbackContext obj)
    { 
        //return _characterControls.Land.Zoom.WasPressedThisFrame();
    }

    public bool ReverseZooming()
    {
        return _characterControls.Land.Zoom.WasReleasedThisFrame();
    }
    
}
