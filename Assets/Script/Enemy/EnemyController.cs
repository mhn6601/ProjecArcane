using UnityEngine;

public class EnemyController : CharacterControllerBase
{
    Entity entity;
    #region StateMachine
    private CharacterStateMachine movementFSM;
    public CharacterStateMachine MovementFSM => movementFSM;
    private CharacterStateMachine actionFSM;
    public CharacterStateMachine ActionFSM => actionFSM;
    #endregion
    #region State
    public EnemyLocomotionState LocomotionState { get; private set; }
    public EnemyEmptyActionState EmptyActionState { get; private set; }
    #endregion

    protected override void Awake()
    {
        entity = GetComponent<Entity>();
        animator = entity.Animator;

        EntityStateMachineInitialize();
        EntityStateInitialize();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementFSM.Initialize(LocomotionState);
        actionFSM.Initialize(EmptyActionState);
    }

    // Update is called once per frame
    void Update()
    {
        movementFSM.Update();
        actionFSM.Update();
    }
    void EntityStateMachineInitialize()
    {
        movementFSM = new CharacterStateMachine();
        actionFSM = new CharacterStateMachine();
    }
    void EntityStateInitialize()
    {
        #region MovementState
        LocomotionState = new EnemyLocomotionState(entity);
        #endregion

        #region ActionState
        EmptyActionState = new EnemyEmptyActionState(entity);
        #endregion 

    }
}
