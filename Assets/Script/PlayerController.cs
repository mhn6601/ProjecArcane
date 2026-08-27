using NUnit.Framework.Interfaces;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : CharacterControllerBase
{
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerJump playerJump;

    private CharacterStateMachine movementFSM { get;  set; }
    private CharacterStateMachine jumpFSM { get;  set; }
    private CharacterStateMachine actionFSM { get;  set; }

    private PlayerInputAction inputActions;
    public PlayerInputAction InputActions => inputActions;

    private Player player;
    public Vector3 moveInput { get; private set; }

    private bool IsRunPressed=false;


    #region State
    [Header("State")]
    public PlayerIdleState IdleState { get;private set; }
    public PlayerWalkState WalkState { get;private set; }
    public PlayerRunState RunState { get;private set; }
    public PlayerJumpState JumpState { get;private set; }
    #endregion

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        inputActions = new PlayerInputAction();

        if (playerMove == null) playerMove = GetComponent<PlayerMove>();
        if (playerJump == null) playerJump = GetComponent<PlayerJump>();

        player=GetComponent<Player>();


        PlayerStateMachineInitialize();
        PlayerStateInitialize();

    }
    private void OnEnable()
    {
        inputActions.PlayerAction.Enable();

        inputActions.PlayerAction.Move.performed += OnMovePerformed;
        inputActions.PlayerAction.Move.canceled += OnMoveCanceled;

        inputActions.PlayerAction.Run.performed += OnRunPerformed;

        inputActions.PlayerAction.Jump.performed += OnJumpPerformed;
    }
    private void OnDisable()
    {
        inputActions.PlayerAction.Move.performed -= OnMovePerformed;
        inputActions.PlayerAction.Move.canceled -= OnMoveCanceled;

        inputActions.PlayerAction.Run.performed -= OnRunPerformed;

        inputActions.PlayerAction.Jump.performed -= OnJumpPerformed;

        inputActions.PlayerAction.Disable();
    }
    private void PlayerStateMachineInitialize()
    {
        movementFSM = new CharacterStateMachine();
        jumpFSM = new CharacterStateMachine();
        actionFSM = new CharacterStateMachine();

    }
    private void PlayerStateInitialize()
    {
        IdleState = new PlayerIdleState(this, player);
        WalkState = new PlayerWalkState(this, player);
        RunState = new PlayerRunState(this, player);
        JumpState = new PlayerJumpState(this, player);
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {

        moveInput = context.ReadValue<Vector3>();
        movementFSM.ChangeState(WalkState);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector3.zero;
        playerMove.ResetCache();// 입력 취소 시 캐시 방향 초기화

        movementFSM.ChangeState(IdleState);
    }
    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsRunPressed = true;

            // 이동 입력이 들어와 있는 상태에서 Shift를 누르면 즉시 RunState로 전환
            if (movementFSM.CurrentState == WalkState && moveInput.sqrMagnitude > 0.001f)
            {
                movementFSM.ChangeState(RunState);
            }
        }
        else if (context.canceled)
        {
            IsRunPressed = false;   

            // 달리던 중 Shift를 떼면 WalkState로 전환
            if (movementFSM.CurrentState == RunState)
            {
                movementFSM.ChangeState(WalkState);
            }
        }
    }
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log(playerJump.IsGround());
        if (playerJump.IsGround())
        {
            playerJump.Jump();
            jumpFSM.ChangeState(JumpState);
        }
    }
    public Vector3 GetJumpVelocity()
    {
        return playerJump.GetJumpVelocity();
    }

}