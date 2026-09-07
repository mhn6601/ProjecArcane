using NUnit.Framework.Interfaces;
using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : CharacterControllerBase
{

   

    #region StateMachine
    private CharacterStateMachine movementFSM;
    public CharacterStateMachine MovementFSM => movementFSM;
    private CharacterStateMachine actionFSM;
    public CharacterStateMachine ActionFSM => actionFSM;
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

    #region State
    [Header("State")]
    public PlayerLocomotionState locomotionState {  get;private set; }
    public PlayerAirborneState AirborneState { get;private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAttackComboState AttackComboState { get;private set; }
    public PlayerHeavyAttackState HeavyAttackState { get;private set; }
    public PlayerParryState ParryState { get;private set; }
    public PlayerEmptyActionState EmptyActionState { get;private set; }
    public PlayerGuardState GuardState { get;private set; }
    #endregion

    [Header("Attack Data Settings")]
    [SerializeField] private PlayerAttackData attackData;


    protected override void Awake()
    {
        animator = Player.Instance.Animator;

        PlayerStateMachineInitialize();
        PlayerStateInitialize();

    }
    private void Start()
    {
        movementFSM.Initialize(locomotionState);
        actionFSM.Initialize(EmptyActionState);
    }
    private void  Update()
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
        #region MovementState
        locomotionState = new PlayerLocomotionState(Player.Instance);
        AirborneState = new PlayerAirborneState(Player.Instance);
        #endregion

        #region ActionState
        JumpState = new PlayerJumpState(Player.Instance);
        AttackComboState = new PlayerAttackComboState(Player.Instance, attackData.lightAttackSteps);
        HeavyAttackState = new PlayerHeavyAttackState(Player.Instance, attackData.heavyAttackSteps);
        ParryState = new PlayerParryState(Player.Instance);
        EmptyActionState = new PlayerEmptyActionState(Player.Instance);
        GuardState = new PlayerGuardState(Player.Instance);
        #endregion 
    }
    private void FSMUpdate()
    {
        movementFSM.Update();
        actionFSM.Update();
    }

    //private void OnMovePerformed(InputAction.CallbackContext context)
    //{
    //    moveInput = context.ReadValue<Vector3>();
    //    if (movementFSM.CurrentState == locomotionState)
    //        movementFSM.ChangeState(locomotionState);
    //}
    //private void OnMoveCanceled(InputAction.CallbackContext context)
    //{
    //    moveInput = Vector3.zero;
    //    //IsRunPressed= false;
    //    //playerMove.ResetCache();// 입력 취소 시 캐시 방향 초기화
    //    if (movementFSM.CurrentState == locomotionState)
    //        movementFSM.ChangeState(locomotionState);
    //}
    //private void OnRunPerformed(InputAction.CallbackContext context)
    //{
    //    IsRunPressed = true;

    //    // 이동 입력이 들어와 있는 상태에서 Shift를 누르면 즉시 RunState로 전환
    //    if (movementFSM.CurrentState == locomotionState && moveInput.sqrMagnitude > 0.001f)
    //    {
    //        movementFSM.ChangeState(locomotionState);
    //    }

    //}
    //private void OnRunCanceled(InputAction.CallbackContext context)
    //{
    //    IsRunPressed = false;
    //    if (movementFSM.CurrentState == locomotionState)
    //    {
    //        movementFSM.ChangeState(locomotionState);
    //    }
    //}
    //private void OnJumpPerformed(InputAction.CallbackContext context)
    //{
    //    if (Status.CanJump)
    //    {
    //        movementFSM.ChangeState(AirborneState);
    //    }
    //}
}