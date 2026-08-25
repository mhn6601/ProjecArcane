using UnityEngine;

public abstract class State : MonoBehaviour
{
    public string stateName;

    public virtual void Initialization() { }
    public abstract void OnEnter();
    public abstract void OnExit();
    
}
