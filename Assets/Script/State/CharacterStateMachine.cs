using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterStateMachine :MonoBehaviour
{
    public CharacterState CurrentState { get; protected set; }
    public CharacterState PreviousState { get; protected set; }

    protected CharacterState newState;

    [SerializeReference] protected List<CharacterState> states;
    //[SerializeField] protected List<GameObject> statesObjects;
    public UnityAction onChangeState;

    private void Awake()
    {
        
    }

    public void Initialize(CharacterState startingState)
    {
        CurrentState = startingState;
        CurrentState.OnEnter();
    }
    public void ChangeState(CharacterState state)
    {
        PreviousState = CurrentState;
        CurrentState?.OnExit();
        newState = FindState(state);
        CurrentState = newState;
        CurrentState.OnEnter();
        onChangeState?.Invoke();
    }
    private CharacterState FindState(CharacterState state)
    {
        if (states != null && state!=null)
        {
            return states.Find(x => x.GetType() == state.GetType());
        }
        Debug.LogError("해당 상태가 없음");
        return null;
    }
}
