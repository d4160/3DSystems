using UnityEngine;

[System.Serializable]
public abstract class CharacterStateBase : StateBase, ICharacterState
{
    public ICharacterMachine CharacterMachine => StateMachine as ICharacterMachine;
    public Animator Animator => CharacterMachine.Animator;
    public Rigidbody Rigidbody => CharacterMachine.Rigidbody;
    public CharacterController CharacterController => CharacterMachine.CharacterController;

    public Vector3 Forward => CharacterController.transform.forward;

    public bool IsGrounded => CharacterMachine.IsGrounded;
    public float Gravity => CharacterMachine.Gravity;
    public Vector3 Velocity { get => CharacterMachine.Velocity; set => CharacterMachine.Velocity = value; }

    public void SetVelocityY(float value)
    {
        Vector3 vel = Velocity;
        vel.y = value;
        Velocity = vel;
    }

    public void AddVelocityY(float value)
    {
        Vector3 vel = Velocity;
        vel.y += value;
        Velocity = vel;
    }

    public virtual void Move()
    {

    }

    public virtual void SwitchRun()
    {

    }

    public virtual void Jump()
    {

    }

    public virtual void Attack()
    {

    }
}

public interface ICharacterState : IState
{
    ICharacterMachine CharacterMachine { get; }

    void Move();
    void SwitchRun();
    void Jump();
    void Attack();
}