using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class StateMachineBase<T> : ComponentBase, IStateMachine<T> where T : IState
{
    protected List<LayerState<T>> _activeStatesByLayer = new();

    public List<LayerState<T>> ActiveStatesByLayer => _activeStatesByLayer;

    [ShowInInspector, TextArea]
    public string ActiveStates
    {
        get
        {
            string debug = string.Empty;
            for (var i = 0; i < _activeStatesByLayer.Count; i++)
            {
                debug += $"Layer: {_activeStatesByLayer[i].layer}, State: {_activeStatesByLayer[i].state}\n";
            }
            return debug;
        }
    }

    public void SetState(T newState)
    {
        if (_activeStatesByLayer.Contains(newState.Layer, out int index))
        {
            _activeStatesByLayer[index].state.OnExit();
            _activeStatesByLayer[index] = new(newState.Layer, newState);
            newState.OnEnter();
        }
        else
        {
            _activeStatesByLayer.Add(new(newState.Layer, newState));
            newState.OnEnter();
        }
    }

    public T GetActiveState(int layer)
    {
        if (_activeStatesByLayer.Contains(layer, out int index))
        {
            return _activeStatesByLayer[index].state;
        }

        return default;
    }

    protected virtual void Update()
    {
        for (var i = 0; i < _activeStatesByLayer.Count; i++)
        {
            _activeStatesByLayer[i].state.Update();
        }
    }

    protected void FixedUpdate()
    {
        for (var i = 0; i < _activeStatesByLayer.Count; i++)
        {
            _activeStatesByLayer[i].state.FixedUpdate();
        }
    }

    protected virtual void OnDrawGizmos()
    {
        for (var i = 0; i < _activeStatesByLayer.Count; i++)
        {
            _activeStatesByLayer[i].state.OnDrawGizmos();
        }
    }
}

public interface IStateMachine<T> : IStateMachine where T : IState
{
    List<LayerState<T>> ActiveStatesByLayer { get; }

    void SetState(T newState);
    T GetActiveState(int layer);
}

public interface IStateMachine
{
}

public struct LayerState<T> : System.IEquatable<LayerState<T>> where T : IState
{
    public int layer;
    public T state;

    public LayerState(int layer, T state)
    {
        this.layer = layer;
        this.state = state;
    }

    public bool Equals(LayerState<T> other) => other.layer == layer;
}

public static class LayerStateExtensions
{
    public static bool Contains<T>(this List<LayerState<T>> instance, int layer, out int index) where T : IState
    {
        for (int i = 0; i < instance.Count; i++)
        {
            if (instance[i].layer == layer)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }
}
