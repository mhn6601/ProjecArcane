using NUnit.Framework.Interfaces;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : CharacterControllerBase
{

    #region Component
    private PlayerMove playerMove;
    public PlayerMove PlayerMove => playerMove;
    private PlayerJump playerJump;
    public PlayerJump PlayerJump => playerJump;
    #endregion

    #region StateMachine
    private CharacterStateMachine movementFSM;
    public CharacterStateMachine MovementFSM => movementFSM;
    private CharacterStateMachine actionFSM;
    public CharacterStateMachine ActionFSM => actionFSM;
    #endregion

    #region InputActions
    private PlayerInputAction inputActions;
    public PlayerInputAction InputActions => inputActions;
    #endregion
    #region ControlDeviceType
    public enum ControlDeviceType
    {
        KeyboardMouse,
        Gamepad
    }
    // 현재 사용 중인 입력 장치 타입 (UI 프롬프트 교체 등에 활용)
    public ControlDeviceType CurrentDevice { get; private set; }
    #endregion

    private Player player;
    public Vector3 moveInput { get; private set; }

    public bool IsRunPressed { get; private set; }


    #region State
    [Header("State")]
    public PlayerLocomotionState locomotionState {  get;private set; }
    public PlayerAirborneState AirborneState { get;private set; }
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
        inputActions.PlayerAction.Run.canceled += OnRunCanceled;

        inputActions.PlayerAction.Jump.performed += OnJumpPerformed;
    }
    private void OnDisable()
    {
        inputActions.PlayerAction.Move.performed -= OnMovePerformed;
        inputActions.PlayerAction.Move.canceled -= OnMoveCanceled;

        inputActions.PlayerAction.Run.performed -= OnRunPerformed;
        inputActions.PlayerAction.Run.canceled -= OnRunCanceled;

        inputActions.PlayerAction.Jump.performed -= OnJumpPerformed;

        inputActions.PlayerAction.Disable();
    }
    private void Start()
    {
        movementFSM.Initialize(locomotionState);
    }
    private void FixedUpdate()
    {
        FSMUpdate();
    }
    // 어떤 장치로 입력했는지 감지하는 메서드
    private void DetectControlDevice(InputDevice device)
    {
        if (device is Gamepad)
        {
            if (CurrentDevice != ControlDeviceType.Gamepad)
            {
                CurrentDevice = ControlDeviceType.Gamepad;
                // Gamepad 전용 UI 버튼 프롬프트 변경 이벤트 실행 가능
            }
        }
        else if (device is Keyboard || device is Mouse)
        {
            if (CurrentDevice != ControlDeviceType.KeyboardMouse)
            {
                CurrentDevice = ControlDeviceType.KeyboardMouse;
                // 키보드/마우스 전용 UI 버튼 프롬프트 변경 이벤트 실행 가능
            }
        }
    }

    private void PlayerStateMachineInitialize()
    {
        movementFSM = new CharacterStateMachine();
        actionFSM = new CharacterStateMachine();

    }
    private void PlayerStateInitialize()
    {
        locomotionState = new PlayerLocomotionState(this);
        AirborneState = new PlayerAirborneState(this);
    }
    private void FSMUpdate()
    {
        movementFSM.Update();
        actionFSM.Update();
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
            moveInput = context.ReadValue<Vector3>();
        if(movementFSM.CurrentState== locomotionState)
            movementFSM.ChangeState(locomotionState);
    }
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector3.zero;
        //IsRunPressed= false;
        //playerMove.ResetCache();// 입력 취소 시 캐시 방향 초기화
        if (movementFSM.CurrentState == locomotionState)
            movementFSM.ChangeState(locomotionState);
    }
    private void OnRunPerformed(InputAction.CallbackContext context)
    {
            IsRunPressed = true;

            // 이동 입력이 들어와 있는 상태에서 Shift를 누르면 즉시 RunState로 전환
            if (movementFSM.CurrentState == locomotionState && moveInput.sqrMagnitude > 0.001f)
            {
                movementFSM.ChangeState(locomotionState);
            }
        
    }
    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        IsRunPressed = false; 
        if (movementFSM.CurrentState == locomotionState)
        {
            movementFSM.ChangeState(locomotionState);
        }
    }
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if(Player.Instance.isGrounded)
        {
            movementFSM.ChangeState(AirborneState);
        }
    }

}