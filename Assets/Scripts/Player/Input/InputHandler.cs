using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public float AttackInput { get; private set; }
    public float BlockInput { get; private set; }
    public float RunInput { get; private set; }
    public float JumpInput { get; private set; }

    public bool canJump;
    public bool canAttack;

    private void Start()
    {
        canJump = true;
        canAttack = true;
    }
    
    public void OnMovementInput(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
    }

    public void OnMouseLookInput(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnLeftClickInput(InputAction.CallbackContext context)
    {
        AttackInput = context.ReadValue<float>();

        if (context.canceled)
            canAttack = true;
    }

    public void OnRightClickInput(InputAction.CallbackContext context)
    {
        BlockInput = context.ReadValue<float>();
    }

    public void OnShiftRunInput(InputAction.CallbackContext context)
    {
        RunInput = context.ReadValue<float>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        JumpInput = context.ReadValue<float>();

        if (context.canceled)
            canJump = true;
    }
}
