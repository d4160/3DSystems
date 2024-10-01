using UnityEngine;

public enum LocomotionType
{
    RootMotion,
    Rigidbody,
    CharacterController
}

[System.Serializable]
public class GroundedLocomotionState : CharacterStateBase
{
    public LocomotionType locomotionType = LocomotionType.CharacterController;
    public float walkingSpeed = 5f;
    public float runningSpeed = 10f;
    public float rotationSpeed = 15f;

    [Header("Params")]
    public string xSpeedParam;
    public string zSpeedParam;

    [Header("Lerp")]
    public float blendSpeedX = 3.31f;
    public float blendSpeedZ = 2.31f;

    private int _xSpeedHash = -1;
    private int _zSpeedHash = -1;

    public int XSpeedHash => _xSpeedHash != -1 ? _xSpeedHash : Animator.StringToHash(xSpeedParam);
    public int ZSpeedHash => _zSpeedHash != -1 ? _zSpeedHash : Animator.StringToHash(zSpeedParam);

    public override void Jump()
    {
        CharacterMachine.SetState(CharacterMachine.JumpState);
    }

    public override void Update()
    {
        Animator.applyRootMotion = locomotionType == LocomotionType.RootMotion;

        float xSpeed = CharacterMachine.Motion.x;
        float zSpeed = CharacterMachine.Motion.y;
        bool running = CharacterMachine.Running;

        float currentXSpeed = Animator.GetFloat(XSpeedHash);
        currentXSpeed = Mathf.Lerp(currentXSpeed, xSpeed, Time.deltaTime * blendSpeedX);

        float currentZSpeed = Animator.GetFloat(ZSpeedHash);
        float targetZSpeed = Mathf.Sign(zSpeed) * Mathf.Min(Mathf.Abs(zSpeed), running ? 1f : 0.5f);
        currentZSpeed = Mathf.Lerp(currentZSpeed, targetZSpeed, Time.deltaTime * blendSpeedZ);

        Animator.SetFloat(XSpeedHash, currentXSpeed);
        Animator.SetFloat(ZSpeedHash, currentZSpeed);

        CharacterControllerMove();
    }

    private void CharacterControllerMove()
    {
        if (locomotionType == LocomotionType.CharacterController)
        {
            if (IsGrounded && Velocity.y < 0)
            {
                SetVelocityY(-2f); // Ensure the player sticks to the ground
            }

            float motionX = CharacterMachine.Motion.x;
            float motionZ = CharacterMachine.Motion.y;
            bool running = CharacterMachine.Running;

            // Motion Move
            CharacterController.Move(Forward * motionZ * (running ? runningSpeed : walkingSpeed) * Time.deltaTime);

            // Rotation
            CharacterController.transform.Rotate(Vector3.up * motionX * rotationSpeed * Time.deltaTime);

            // Gravity
            AddVelocityY(Gravity * Time.deltaTime);

            // Y Move
            CharacterController.Move(Velocity * Time.deltaTime);
        }
    }

    public override void FixedUpdate()
    {
        if (locomotionType == LocomotionType.Rigidbody)
        {
            float xSpeed = CharacterMachine.Motion.x;
            float zSpeed = CharacterMachine.Motion.y;
            bool running = CharacterMachine.Running;

            // Motion
            Rigidbody.linearVelocity = Rigidbody.transform.forward * zSpeed * (running ? runningSpeed : walkingSpeed);

            // Rotation
            Rigidbody.MoveRotation(Rigidbody.rotation * Quaternion.Euler(Vector3.up * xSpeed * rotationSpeed * Time.fixedDeltaTime));
        }
    }
}
