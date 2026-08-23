using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class CharacterControllerBase : MonoBehaviour
{
    protected CharacterController characterController;
    protected CharacterStateMachine stateMachine;
    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public virtual void ChangeState(CharacterState state)
    {
        stateMachine?.ChangeState(state);
    }
}
