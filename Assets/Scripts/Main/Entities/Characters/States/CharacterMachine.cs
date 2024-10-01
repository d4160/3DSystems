using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterMachine : StateMachineBase<ICharacterState>, ICharacterMachine
{
    public float gravity = -9.81f;
    public string isGroundedParam = "IsGrounded";

    [Header("References")]
    [SerializeField] public Animator _animator;
    [SerializeField] public Rigidbody _rigidbody;
    [SerializeField] public CharacterController _characterController;

    [Header("Available States")]
    public GroundedLocomotionState locomotionState;
    public JumpState jumpState;
    public AttackState attackState;

    [Header("Runtime Values")]
    [SerializeField] private Vector2 _motion;
    [SerializeField] private bool _running;
    [SerializeField] private bool _isGrounded;
    [SerializeField] private Vector3 _velocity;

    private int _isGroundedParamHash;

    public Animator Animator => _animator;
    public Rigidbody Rigidbody => _rigidbody;
    public CharacterController CharacterController => _characterController;

    public ICharacterState LocomotionState => locomotionState;
    public ICharacterState JumpState => jumpState;

    public Vector2 Motion => _motion;
    public bool Running => _running;
    public bool IsGrounded => _isGrounded;
    public float Gravity => gravity;
    public Vector3 Velocity { get => _velocity; set => _velocity = value; }

    void Awake()
    {
        if (!_animator)
        {
            _animator = GetComponent<Animator>();
        }

        if (!_rigidbody)
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        if (!_characterController)
        {
            _characterController = GetComponent<CharacterController>();
        }
    }

    void Start()
    {
        _isGroundedParamHash = Animator.StringToHash(isGroundedParam);

        locomotionState.StateMachine = this;
        jumpState.StateMachine = this;
        attackState.StateMachine = this;

        SetState(locomotionState);
        SetState(attackState);
    }

    protected override void Update()
    {
        _isGrounded = _characterController.isGrounded;
        Animator.SetBool(_isGroundedParamHash, _isGrounded);

        base.Update();
    }

    public void Move(Vector2 motion)
    {
        _motion = motion;
        GetActiveState(locomotionState.Layer).Move();
    }

    public void SwitchRun(bool run)
    {
        _running = run;
        GetActiveState(locomotionState.Layer).SwitchRun();
    }

    public void Jump()
    {
        GetActiveState(locomotionState.Layer).Jump();
    }

    public void Attack()
    {
        GetActiveState(attackState.Layer).Attack();
    }
}

public interface ICharacterMachine : IStateMachine<ICharacterState>
{
    Animator Animator { get; }
    Rigidbody Rigidbody { get; }
    CharacterController CharacterController { get; }

    ICharacterState JumpState { get; }
    ICharacterState LocomotionState { get; }

    Vector2 Motion { get; }
    bool Running { get; }
    bool IsGrounded { get; }
    Vector3 Velocity { get; set; }
    float Gravity { get; }

    void Move(Vector2 motion);
    void SwitchRun(bool run);
    void Jump();
    void Attack();
}
