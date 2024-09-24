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
}
