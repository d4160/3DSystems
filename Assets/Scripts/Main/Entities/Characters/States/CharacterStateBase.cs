using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public abstract class CharacterStateBase : StateBase, ICharacterState
{
    public CharacterMachine CharacterMachine => StateMachine as CharacterMachine;

    public virtual void Move(Vector2 movement)
    {

    }
}

public interface ICharacterState
{
    CharacterMachine CharacterMachine { get; }

    void Move(Vector2 movement);
}