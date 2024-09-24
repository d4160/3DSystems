using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public abstract class StateBase : IState
{
    [SerializeField] protected int _layer;

    private IStateMachine _stateMachine;

    public int Layer => _layer;

    public IStateMachine StateMachine
    {
        get => _stateMachine;
        set
        {
            Start();
            _stateMachine = value;
        }
    }

    /// <summary>
    /// Only called once
    /// </summary>
    public virtual void Start()
    {

    }

    public virtual void OnEnter()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void OnExit()
    {

    }
}

public interface IState
{
    int Layer { get; }
    IStateMachine StateMachine { get; set; }

    void Start();
    void OnEnter();
    void Update();
    void OnExit();
}
