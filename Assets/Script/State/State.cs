using UnityEngine;

public abstract class State 
{
    public string stateName;

    public virtual void Initialization() { }
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void Update();
}
