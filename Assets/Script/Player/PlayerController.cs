using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : CharacterControllerBase
{

    Player player;

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
    public PlayerRollState RollState { get; private set; }
    public PlayerAttackComboState AttackComboState { get;private set; }
    public PlayerHeavyAttackState HeavyAttackState { get;private set; }
    public PlayerParryState ParryState { get;private set; }
    public PlayerEmptyActionState EmptyActionState { get;private set; }
    public PlayerGuardState GuardState { get;private set; }
    public PlayerHitStunState HitStunState { get;private set; }
    #endregion



    protected override void Awake()
    {
        player = Player.Instance;
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
        locomotionState = new PlayerLocomotionState(player);
        AirborneState = new PlayerAirborneState(player);
        RollState = new PlayerRollState(player);
        #endregion

        #region ActionState
        JumpState = new PlayerJumpState(player);
        AttackComboState = new PlayerAttackComboState(player, player.Attack.GetLightAttackStepData());
        HeavyAttackState = new PlayerHeavyAttackState(player, player.Attack.GetHeavyAttackStepData());

        ParryState = new PlayerParryState(player);
        EmptyActionState = new PlayerEmptyActionState(player);
        GuardState = new PlayerGuardState(player);
        HitStunState = new PlayerHitStunState(player);
        #endregion 
    }
    private void FSMUpdate()
    {
        movementFSM.Update();
        actionFSM.Update();
    }
}