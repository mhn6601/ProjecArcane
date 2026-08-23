using UnityEngine;
using UnityEngine.Events;

public interface  CharacterState 
{
    //protected CharacterControllerBase characterControllerBase;
    //protected CharacterStateMachine characterStateMachine;
    //public CharacterState(CharacterControllerBase characterControllerBase, CharacterStateMachine characterStateMachine)
    //{
    //    this.characterControllerBase = characterControllerBase;
    //    this.characterStateMachine = characterStateMachine;
    //}

    void OnEnter() { }
    void OnExit() { }
    void FixedUpdate() { } // FixedUpdate/Move Ã³¸®
}
