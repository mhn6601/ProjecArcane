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
    public PlayerSpellCastState SpellCastState { get;private set; }
    #endregion



    protected override void Awake()
    {
        player = Player.Instance;
        

       

    }
    private void Start()
    {
        animator = Player.Instance.Animation.Animator;
        PlayerStateMachineInitialize();
        PlayerStateInitialize();
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
        locomotionState = new PlayerLocomotionState(player, animator);
        AirborneState = new PlayerAirborneState(player, animator);
        RollState = new PlayerRollState(player, animator);
        #endregion

        #region ActionState
        JumpState = new PlayerJumpState(player, animator);
        AttackComboState = new PlayerAttackComboState(player, animator, player.Attack.GetLightAttackStepData());
        HeavyAttackState = new PlayerHeavyAttackState(player, animator, player.Attack.GetHeavyAttackStepData());

        ParryState = new PlayerParryState(player, animator);
        EmptyActionState = new PlayerEmptyActionState(player, animator);
        GuardState = new PlayerGuardState(player,animator);
        HitStunState = new PlayerHitStunState(player, animator);
        SpellCastState= new PlayerSpellCastState(player, animator,null);
        #endregion
    }
    private void FSMUpdate()
    {
        movementFSM.Update();
        actionFSM.Update();
    }
    public void OnMainWeaponChange(PlayerAttackData playerAttackData)
    {
        AttackComboState = new PlayerAttackComboState(player,animator, playerAttackData.lightAttackSteps);
        HeavyAttackState = new PlayerHeavyAttackState(player, animator,playerAttackData.heavyAttackSteps);
    }
    public void OnSubWeaponChange(PlayerAttackData playerAttackData)
    {
    }
}