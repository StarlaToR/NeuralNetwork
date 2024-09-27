using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Character
{
    private void Update()
    {
        HandleInput();
    }

    public void MoveInput(InputAction.CallbackContext ctx)
    {
        movementDirection = ctx.ReadValue<float>();
    }

    public void JumpInput(InputAction.CallbackContext ctx)
    {
        if (isGrounded && ctx.started)
            Jump();
    }

    void HandleInput()
    {
        Move(movementDirection);
    }
}
