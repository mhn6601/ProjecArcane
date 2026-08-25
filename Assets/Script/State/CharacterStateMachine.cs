using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStateMachine :MonoBehaviour
{
    public State CurrentState { get; protected set; }
    public State PreviousState { get; protected set; }

    protected State newState;

    [SerializeField]
    protected List<State> states;
    //[SerializeField] protected List<GameObject> statesObjects;
    public UnityAction onChangeState;

    private void Awake()
    {
        if (states.Count == 0)
        {
            var _states = GetComponents<State>();

            if (_states != null)
            {
                states.AddRange(_states);
            }
        }
        foreach (var state in states)
        {
            state.Initialization();
        }
        Initialize(states[0]);
    }

    public void Initialize(State startingState)
    {   
        CurrentState = startingState;
        CurrentState.OnEnter();
    }
    public void ChangeState(State state)
    {
        PreviousState = CurrentState;
        CurrentState?.OnExit();
        newState = FindState(state);
        CurrentState = newState;
        CurrentState.OnEnter();
        onChangeState?.Invoke();
    }
    public void ChangeState<T>(T state) where T : State
    {
        PreviousState = CurrentState;
        CurrentState?.OnExit();
        newState = FindState(state);
        CurrentState = newState;
        CurrentState.OnEnter();
        onChangeState?.Invoke();
    }
    public void ChangeState(string stateName)
    {
        PreviousState = CurrentState;
        CurrentState?.OnExit();
        newState = FindState(stateName);
        CurrentState = newState;
        CurrentState.OnEnter();
        onChangeState?.Invoke();
    }
    private State FindState(State targetState)
    {
        if (states != null && states.Count > 0 && !string.IsNullOrEmpty(targetState.stateName))
        {
            foreach (var state in states)
            {
                if (state.stateName == targetState.stateName)
                {
                    return state;
                }
            }
        }
        else if (!string.IsNullOrEmpty(targetState.stateName))
        {

            Debug.LogError("해당 상태의 Character Controller 컴포넌트에 이름이 설정되지 않음");
        }
        Debug.LogError("해당 상태가 없음");
        return null;
    }
    private State FindState(string stateName)
    {
        if (states != null && states.Count > 0 && !string.IsNullOrEmpty(stateName))
        {
            foreach (var state in states)
            {
                if (state.stateName == stateName)
                {
                    return state;
                }
            }
        }
        Debug.LogError("해당 상태가 없음");
        return null;
    }
}
