using UnityEngine;

[System.Serializable]
public class JumpState : CharacterStateBase
{
    public LocomotionType locomotionType = LocomotionType.CharacterController;
    public float jumpForce = 10f;
    public float jumpHeight = 2f;
    public float onAirSpeed = 3.1f;

    [Header("Params")]
    public string jumpParam;

    [Header("State")]
    public string jumpState = "Jump";

    private int _jumpParamHash;
    private bool _canExitState;
    private bool _jumped;

    public override void Start()
    {
        _jumpParamHash = Animator.StringToHash(jumpParam);
    }

    public override void OnEnter()
    {
        _jumped = false;

        Animator.applyRootMotion = locomotionType == LocomotionType.RootMotion;

        Animator.SetTrigger(_jumpParamHash);
    }

    public override void Jump()
    {
        if (_jumped) return;

        if (IsGrounded)
        {
            switch (locomotionType)
            {
                case LocomotionType.CharacterController:
                    SetVelocityY(Mathf.Sqrt(jumpHeight * -2f * Gravity));
                    break;
                case LocomotionType.Rigidbody:
                    Rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                    break;
            }

            CharacterMachine.IsGrounded = false;
        }

        _jumped = true;
    }

    public override void Update()
    {
        Animator.applyRootMotion = locomotionType == LocomotionType.RootMotion;

        AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(Layer);

        if (_canExitState && !Animator.IsInTransition(Layer) && !stateInfo.IsName(jumpState))
        {
            CharacterMachine.SetState(CharacterMachine.LocomotionState);
            _canExitState = false;
        }
        else if (Animator.IsInTransition(Layer) && stateInfo.IsName(jumpState))
        {
            _canExitState = true;
        }

        CharacterControllerMove();
    }

    private void CharacterControllerMove()
    {
        if (locomotionType == LocomotionType.CharacterController)
        {

            float motionZ = CharacterMachine.Motion.y;

            // Motion Move
            CharacterController.Move(Forward * motionZ * onAirSpeed * Time.deltaTime);

            // Gravity
            AddVelocityY(Gravity * Time.deltaTime);

            // Y Move
            CharacterController.Move(Velocity * Time.deltaTime);
        }
    }
}
