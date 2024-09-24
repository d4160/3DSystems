using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class GroundedLocomotionState : CharacterStateBase
{
    [Header("Params")]
    public string xSpeedParam;
    public string zSpeedParam;

    [Header("Runtime Values")]
    [SerializeField] private bool _running;
    [SerializeField] private float _xSpeed;
    [SerializeField] private float _zSpeed;

    private int _xSpeedHash = -1;
    private int _zSpeedHash = -1;

    public int XSpeedHash => _xSpeedHash != -1 ? _xSpeedHash : Animator.StringToHash(xSpeedParam);
    public int ZSpeedHash => _zSpeedHash != -1 ? _zSpeedHash : Animator.StringToHash(zSpeedParam);

    public Animator Animator => CharacterMachine.Animator;

    public override void Move(Vector2 movement)
    {
        _xSpeed = movement.x;
        _zSpeed = movement.y;
    }

    public override void Update()
    {
        Animator.SetFloat(XSpeedHash, _xSpeed);
        Animator.SetFloat(ZSpeedHash, Mathf.Min(_zSpeed, _running ? 1f : 0.5f));
    }
}
