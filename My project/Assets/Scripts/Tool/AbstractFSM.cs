using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractFSM : MonoBehaviour
{
    protected AbstractStates currentState;
    protected Dictionary<Type, AbstractStates> states = new();

    protected virtual void Update() { currentState?.OnUpdate(); }
    protected virtual void FixedUpdate() { currentState?.OnFixedUpdate(); }
    protected virtual void OnDestroy() { currentState?.OnDestroy(); }
    protected virtual void OnCollisionEnter(Collision collision) { currentState?.OnCollisionEnter(collision); }
    public virtual void OnTriggerEnter(Collider other) { currentState?.OnTriggerEnter(other); }
    public virtual void OnTriggerExit(Collider other) { currentState?.OnTriggerExit(other); }

    public void AddState<T>(AbstractStates state) where T : AbstractStates
    {
        if (!states.ContainsKey(typeof(T)))
            states.Add(typeof(T), state);
    }
    public void SwitchState<T>() where T : AbstractStates
    {
        if (!states.ContainsKey(typeof(T))) return;
        currentState?.OnExit();
        currentState = states[typeof(T)];
        currentState.OnEnter();
    }
}

public abstract class AbstractStates
{
    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
    public virtual void OnExit() { }
    public virtual void OnDestroy() { }
    public virtual void OnCollisionEnter(Collision collision) { }
    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void OnTriggerExit(Collider other) { }
}