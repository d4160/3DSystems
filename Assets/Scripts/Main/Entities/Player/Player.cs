using UnityEngine;
using UnityEngine.InputSystem;

public class Player : PlayerBase
{
    public CharacterMachine character;

    public void Move(InputAction.CallbackContext ctx)
    {
        var movement = ctx.ReadValue<Vector2>();
        character.Move(movement);
    }

    public void SwitchRun(InputAction.CallbackContext ctx)
    {
        bool run = ctx.performed;

        character.SwitchRun(run);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            character.Jump();
        }
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            character.Attack();
        }
    }
}
