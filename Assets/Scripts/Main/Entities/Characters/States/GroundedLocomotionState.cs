using UnityEngine;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

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
    public float maxStepHeight = 0.7f;
    public LayerMask groundMask;
    public float raySpacing = 0.1f;

    [Header("Params")]
    public string xSpeedParam;
    public string zSpeedParam;

    [Header("Lerp")]
    public float blendSpeedX = 3.31f;
    public float blendSpeedZ = 2.31f;
    public float positionFixerSpeed = 10f;

    private int _xSpeedHash = -1;
    private int _zSpeedHash = -1;

    public int XSpeedHash => _xSpeedHash != -1 ? _xSpeedHash : Animator.StringToHash(xSpeedParam);
    public int ZSpeedHash => _zSpeedHash != -1 ? _zSpeedHash : Animator.StringToHash(zSpeedParam);

    public override void Jump()
    {
        CharacterMachine.SetState(CharacterMachine.JumpState);
    }


    public class Character
    {
        public string name;
        public int age;
        public float money;
    }

    [Button]
    public void NewCharacter()
    {
        Character c = new Character() { name = "Alejo", age = 20, money = 999999 };
        Debug.Log(JsonConvert.SerializeObject(c, Formatting.Indented));
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

    RaycastHit _hit;

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

            // Positions of the 3 rays
            Vector3 rayOrigin = CharacterController.transform.position + CharacterController.transform.up * maxStepHeight;
            Vector3[] rayOrigins = new Vector3[3];
            rayOrigins[0] = rayOrigin + CharacterController.transform.right * -raySpacing;
            rayOrigins[1] = rayOrigin;
            rayOrigins[2] = rayOrigin + CharacterController.transform.right * raySpacing;

            float maxYDistance = float.MinValue;
            for (int i = 0; i < rayOrigins.Length; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(rayOrigins[i], Vector3.down, out hit, 100f, groundMask.value))
                {
                    if (hit.distance > maxYDistance)
                    {
                        maxYDistance = hit.distance;
                        _hit = hit;
                    }
                }
            }

            if (maxYDistance == float.MinValue)
            {
                CharacterMachine.IsGrounded = false;
                AddVelocityY(Gravity * Time.deltaTime); // Apply gravity if no ground detected
            }
            else
            {
                float distanceToGround = maxYDistance;
                if (distanceToGround < maxStepHeight + 0.01f)
                {
                    CharacterMachine.IsGrounded = true;
                    Vector3 fixedPos = CharacterController.transform.position;
                    fixedPos.y = _hit.point.y;
                    CharacterController.enabled = false;

                    CharacterController.transform.position = Vector3.Lerp(CharacterController.transform.position, fixedPos, Time.deltaTime * positionFixerSpeed);
                    CharacterController.enabled = true;
                    SetVelocityY(0); // No gravity
                }
                else
                {
                    CharacterMachine.IsGrounded = false;
                    AddVelocityY(Gravity * Time.deltaTime); // Apply gravity
                }
            }


            // Motion Move
            CharacterController.Move(Forward * motionZ * (running ? runningSpeed : walkingSpeed) * Time.deltaTime);

            // Rotation
            CharacterController.transform.Rotate(Vector3.up * motionX * rotationSpeed * Time.deltaTime);

            // Y Move
            CharacterController.Move(Velocity * Time.deltaTime);
        }
    }

    public override void OnDrawGizmos()
    {
        Vector3 rayOrigin = CharacterController.transform.position + CharacterController.transform.up * maxStepHeight;
        Vector3[] rayOrigins = new Vector3[3];
        rayOrigins[0] = rayOrigin + CharacterController.transform.right * -raySpacing;
        rayOrigins[1] = rayOrigin;
        rayOrigins[2] = rayOrigin + CharacterController.transform.right * raySpacing;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(rayOrigins[0], rayOrigins[0] + Vector3.down * (maxStepHeight + 0.1f));
        Gizmos.DrawLine(rayOrigins[1], rayOrigins[1] + Vector3.down * (maxStepHeight + 0.1f));
        Gizmos.DrawLine(rayOrigins[2], rayOrigins[2] + Vector3.down * (maxStepHeight + 0.1f));
        Gizmos.DrawWireSphere(_hit.point, 0.1f);

        Vector3 fixedPos = CharacterController.transform.position;
        fixedPos.y = _hit.point.y + maxStepHeight + 0.05f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(fixedPos, 0.1f);
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
