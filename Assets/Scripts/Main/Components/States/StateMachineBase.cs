using System.Collections.Generic;

public class StateMachineBase<T> : ComponentBase, IStateMachine<T> where T : IState
{
    protected Dictionary<int, T> _activeStatesByLayer = new();

    public Dictionary<int, T> ActiveStatesByLayer => _activeStatesByLayer;

    public virtual void SetState(T newState)
    {
        if (_activeStatesByLayer.ContainsKey(newState.Layer))
        {
            _activeStatesByLayer[newState.Layer].OnExit();
            _activeStatesByLayer[newState.Layer] = newState;
            newState.OnEnter();
        }
        else
        {
            _activeStatesByLayer.Add(newState.Layer, newState);
            newState.OnEnter();
        }
    }

    public T GetActiveState(int layer)
    {
        if (_activeStatesByLayer.ContainsKey(layer))
        {
            return _activeStatesByLayer[layer];
        }

        return default;
    }

    protected virtual void Update()
    {
        foreach (var state in _activeStatesByLayer)
        {
            state.Value.Update();
        }
    }
}

public interface IStateMachine<T> : IStateMachine where T : IState
{
    Dictionary<int, T> ActiveStatesByLayer { get; }

    T GetActiveState(int layer);
}

public interface IStateMachine
{
}
