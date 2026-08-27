using UnityEngine;
[RequireComponent(typeof(CharacterController),typeof(Animator))]
public class CharacterControllerBase : MonoBehaviour
{
    protected CharacterController characterController;
    public CharacterController CC
    {
        get
        {
            if(characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }
            return characterController;
        }
        
    }

    protected CharacterStateMachine stateMachine;

    [SerializeField]
    protected Animator animator;
    public Animator Animator => animator;
    protected virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public virtual void ChangeState(State state)
    {
        stateMachine?.ChangeState(state);
    }
    public virtual void ChangeState<T>(T state) where T : State
    {
        stateMachine?.ChangeState<T>(state);
    }
}
