using UnityEngine;

public class EnemyController : CharacterControllerBase
{
    Enemy enemy;
    #region StateMachine
    private CharacterStateMachine movementFSM;
    public CharacterStateMachine MovementFSM => movementFSM;
    private CharacterStateMachine actionFSM;
    public CharacterStateMachine ActionFSM => actionFSM;
    #endregion
    #region State
    public EnemyLocomotionState LocomotionState { get; private set; }
    public EnemyEmptyActionState EmptyActionState { get; private set; }
    public EnemyHitStunState HitStunState { get; private set; }
    #endregion

    protected override void Awake()
    {
        enemy = GetComponent<Enemy>();
        

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = enemy.Animation.Animator;

        EntityStateMachineInitialize();
        EntityStateInitialize();

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
        LocomotionState = new EnemyLocomotionState(enemy, animator);
        #endregion

        #region ActionState
        EmptyActionState = new EnemyEmptyActionState(enemy,animator);
        #endregion 

    }
}
