using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStateMachine 
{
    public State CurrentState { get; protected set; }
    public State PreviousState { get; protected set; }

    protected State newState;

    [SerializeField]
    protected List<State> states;
    //[SerializeField] protected List<GameObject> statesObjects;
    public UnityAction onChangeState;

    public void Initialize(State startingState)
    {
        CurrentState = startingState;
        CurrentState.OnEnter();
    }
    public void ChangeState(State state)
    {
        if (CurrentState == state) return;
        PreviousState = CurrentState;
        CurrentState?.OnExit();
        newState = state;
        CurrentState = newState;
        CurrentState.OnEnter();
        onChangeState?.Invoke();
    }
    public void ChangeState<T>(T state) where T : State
    {
        if (CurrentState == state) return;
        PreviousState = CurrentState;
        CurrentState?.OnExit();
        newState = state;
        CurrentState = newState;
        CurrentState.OnEnter();
        onChangeState?.Invoke();
    }
    public void Update()
    {
        CurrentState?.Update();
    }   
}
