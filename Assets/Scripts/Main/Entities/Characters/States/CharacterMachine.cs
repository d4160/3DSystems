using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterMachine : StateMachineBase<CharacterStateBase>
{
    [Header("References")]
    [SerializeField] public Animator _animator;

    [Header("Available States")]
    public GroundedLocomotionState locomotionState;

    public Animator Animator => _animator;

    void Awake()
    {
        if (!_animator)
        {
            _animator = GetComponent<Animator>();
        }
    }

    void Start()
    {
        locomotionState.StateMachine = this;

        SetState(locomotionState);
    }

    public void Move(Vector2 movement)
    {
        GetActiveState(locomotionState.Layer).Move(movement);
    }
}
