
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private UIManager uiManager;
    private PlayerInputAction inputActions;
    [SerializeField]private CinemachineInputAxisController cinemachineInputAxisController;

    private Dictionary<InputAction, UIType> uiToggleMap;
    public  Action<UIType> OnUIToggleInput;
    public Action<Vector2> OnLookInput;
    public Action<bool> OnInputEnabledChanged;
    public Action OnLockOnClicked;
    public Action<int> OnSpellInput;
    private const float SpellBufferTime = 0.2f;
    private int bufferedSpellSlot = -1;
    private float bufferedSpellTime;
    private InputAction[] spellActions;
    private Action<InputAction.CallbackContext>[] spellCallbacks;
    private bool inputEnabled = true;
    public bool InputEnabled =>inputEnabled;
    public Vector3 MoveInput { get; private set; }
    public bool IsRunPressed { get; private set; }
    public bool IsGuardHold { get; private set; }

    public bool RollTriggered { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool AttackTriggered { get; private set; }
    public bool HeavyAttackTriggered { get; private set; }
    public bool ParryTriggered { get; private set; }
    public bool IsLockedOn { get; private set; }

    // ★ 구르기 트리거 시점에 달리는 중이었는지 여부. RollState에서 애니메이션 분기에 사용.
    public bool RollWhileRunning { get; private set; }

    [Header("공유 키 모드 전용 (Run/Roll이 같은 키일 때만 사용됨)")]
    [Tooltip("이 시간 이상 누르고 있으면 '달리기'로 전환합니다.")]
    [SerializeField] private float holdThreshold = 0.2f;

    [Tooltip("달리기 도중 손을 뗀 뒤, 이 시간 안에 다시 누르면 즉시 구르기(달리다 구르기)로 확정합니다.")]
    [SerializeField] private float runCancelRollBufferTime = 0.15f;

    private float pressStartTime;
    private bool isHeld;
    private bool hasBecomeRun;

    private bool justStoppedRunning;
    private float justStoppedRunningTime;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
        cinemachineInputAxisController = FindAnyObjectByType<CinemachineInputAxisController>();
    }

    private void OnEnable()
    {
        #region Player Action

        inputActions.PlayerAction.Enable();

        inputActions.PlayerAction.Move.performed += OnMovePerformed;
        inputActions.PlayerAction.Move.canceled += OnMoveCanceled;
        inputActions.PlayerAction.Run.started += OnRunStarted;
        inputActions.PlayerAction.Run.canceled += OnRunCanceled;

        inputActions.PlayerAction.Roll.performed += OnRollPerformed;

        inputActions.PlayerAction.Jump.performed += OnJumpPerformed;
        inputActions.PlayerAction.Attack.performed += OnAttackPerformed;
        inputActions.PlayerAction.HeavyAttack.performed += OnHeavyAttackPerformed;
        inputActions.PlayerAction.Parry.performed += OnParryPerformed;
        inputActions.PlayerAction.Guard.performed += OnGuardHoldPerformed;
        inputActions.PlayerAction.Guard.canceled += OnGuardHoldCanceled;
        BindSpellInputs();
        #endregion

        #region Player Caemra
        inputActions.PlayerCamera.Enable();

        inputActions.PlayerCamera.Rotate.performed += OnLookPerformed;
        inputActions.PlayerCamera.LockOn.performed += OnLockOnPerformed;
        inputActions.PlayerCamera.LockOnTargetSwitch.performed += OnLockOnTargetSwitched;

        #endregion

        #region Player UI
        inputActions.PlayerUI.Enable();

        uiToggleMap = new Dictionary<InputAction, UIType>
{
    { inputActions.PlayerUI.Inventory, UIType.Inventory },
    { inputActions.PlayerUI.Spell,     UIType.Spell },
    // 새 UI는 여기에 한 줄만 추가
};

        foreach (var pair in uiToggleMap)
            pair.Key.performed += OnUITogglePerformed;
        #endregion



    }

    private void OnDisable()

    {
        #region Player Action
        inputActions.PlayerAction.Move.performed -= OnMovePerformed;
        inputActions.PlayerAction.Move.canceled -= OnMoveCanceled;

        inputActions.PlayerAction.Run.started -= OnRunStarted;
        inputActions.PlayerAction.Run.canceled -= OnRunCanceled;

        inputActions.PlayerAction.Roll.performed -= OnRollPerformed;

        inputActions.PlayerAction.Jump.performed -= OnJumpPerformed;
        inputActions.PlayerAction.Attack.performed -= OnAttackPerformed;
        inputActions.PlayerAction.HeavyAttack.performed -= OnHeavyAttackPerformed;
        inputActions.PlayerAction.Parry.performed -= OnParryPerformed;
        inputActions.PlayerAction.Guard.performed -= OnGuardHoldPerformed;
        inputActions.PlayerAction.Guard.canceled -= OnGuardHoldCanceled;
        UnbindSpellInputs();
        inputActions.PlayerAction.Disable();
        #endregion

        #region Player Caemra

        inputActions.PlayerCamera.Rotate.performed -= OnLookPerformed;
        inputActions.PlayerCamera.LockOn.performed -= OnLockOnPerformed;
        inputActions.PlayerCamera.LockOnTargetSwitch.performed -= OnLockOnTargetSwitched;

        inputActions.PlayerCamera.Disable();
        #endregion

        #region Player UI
        if (uiToggleMap != null)
        {
            foreach (var pair in uiToggleMap)
                pair.Key.performed -= OnUITogglePerformed;
        }
        inputActions.PlayerUI.Disable();
        #endregion



    }
    private void Start()
    {
        uiManager = UIManager.Instance;
        if(uiManager!=null)
        uiManager.OnPlayerInputChanged += SetInputEnabled;
    }
    private void OnDestroy()
    {
        if (uiManager != null)
            uiManager.OnPlayerInputChanged -= SetInputEnabled;
    }
    public void SetInputEnabled(bool enabled)
    {
        if (inputEnabled == enabled)
            return;

        inputEnabled = enabled;

        if (!enabled)
        {
            MoveInput = Vector3.zero;
            IsRunPressed = false;
            IsGuardHold = false;
            bufferedSpellSlot = -1;
            cinemachineInputAxisController.enabled = false;
        }
        else
        {

            cinemachineInputAxisController.enabled = true;
        }

        OnInputEnabledChanged?.Invoke(inputEnabled);
    }
    #region Move input handling
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (inputEnabled)
        {
            MoveInput = context.ReadValue<Vector3>();
        }
        else
        {
            MoveInput = Vector3.zero;

        }
    }
    private void OnMoveCanceled(InputAction.CallbackContext context) => MoveInput = Vector3.zero;
    #endregion
    #region Run input handling
    private void OnRunStarted(InputAction.CallbackContext context)
    {
        if (!inputEnabled)
        {

            OnRunCanceled(context);
            return;
        }

        float now = Time.unscaledTime;

        if (IsSharedKeyMode())
        {
            // 공유 키 모드: 탭/홀드/재입력 판정을 Run 이벤트 타이밍으로 전담 처리
            if (justStoppedRunning && (now - justStoppedRunningTime) <= runCancelRollBufferTime)
            {
                TriggerRoll(wasRunning: true);
                justStoppedRunning = false;
            }
            pressStartTime = now;
            isHeld = true;
            hasBecomeRun = false;
        }
        else
        {
            // 전용 달리기 키 모드: 누르는 즉시 달리기 시작 (탭 판정 불필요, 구르기는 별도 키)
            IsRunPressed = true;
        }
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        float now = Time.unscaledTime;

        if (IsSharedKeyMode())
        {
            float holdDuration = now - pressStartTime;

            if (!hasBecomeRun && holdDuration < holdThreshold)
            {
                TriggerRoll(wasRunning: false);
            }

            if (hasBecomeRun)
            {
                justStoppedRunning = true;
                justStoppedRunningTime = now;
            }

            isHeld = false;
            hasBecomeRun = false;
        }

        IsRunPressed = false;
    }
    #endregion
    #region Roll input handling
    /// <summary>
    /// Run 액션과 Roll 액션이 현재(리바인딩 반영된) 상태에서 같은 물리 컨트롤을 가리키는지 실시간 판정.
    /// 캐시하지 않고 그때그때 계산하므로, 리바인딩 메뉴에서 무엇을 바꾸든 별도 갱신 호출 없이 항상 최신 상태를 반영합니다.
    /// </summary>
    private bool IsSharedKeyMode()
    {
        var runControls = inputActions.PlayerAction.Run.controls;
        var rollControls = inputActions.PlayerAction.Roll.controls;

        for (int i = 0; i < runControls.Count; i++)
        {
            for (int j = 0; j < rollControls.Count; j++)
            {
                if (runControls[i] == rollControls[j])
                    return true;
            }
        }
        return false;
    }

   

    private void OnRollPerformed(InputAction.CallbackContext context)
    {
        if (!inputEnabled)
            return;
        // 공유 키 모드에서는 Run 쪽 press/release 타이밍 로직이 구르기 판정을 전담하므로
        // Roll 액션 자체의 즉시 발동(performed)은 무시해야 이중 발동을 막을 수 있음.
        if (IsSharedKeyMode())
            return;

        // 전용 구르기 키 모드: 누르는 즉시 구르기. 이 순간 달리는 중이었다면 '달리며 구르기'.
        TriggerRoll(wasRunning: IsRunPressed);
    }

    private void TriggerRoll(bool wasRunning)
    {
        RollTriggered = true;
        RollWhileRunning = wasRunning;
    }
    #endregion
   
    #region Jump input handling
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return; 
        JumpTriggered = true;
    }
    #endregion
    #region Attack input handling
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!inputEnabled) return; 
        AttackTriggered = true;
    }
    private void OnHeavyAttackPerformed(InputAction.CallbackContext context) 
    { 
        if (!inputEnabled) return; 
        HeavyAttackTriggered = true;
    }
    #endregion
    #region Parry input handling

    private void OnParryPerformed(InputAction.CallbackContext context) 
    {
        if (!inputEnabled) return; 
        ParryTriggered = true; 
    }
    #endregion 
    #region Guard input handling
    private void OnGuardHoldPerformed(InputAction.CallbackContext context) 
    {
        if (!inputEnabled) return; 
        IsGuardHold = true;
    }
    private void OnGuardHoldCanceled(InputAction.CallbackContext context) 
    {
        if (!inputEnabled) return; 
        IsGuardHold = false; 
    }
    #endregion
    #region Spell Input Handling
    private void UnbindSpellInputs()
    {
        if (spellActions == null) return;

        for (int i = 0; i < spellActions.Length; i++)
            spellActions[i].performed -= spellCallbacks[i];

        spellActions = null;
        spellCallbacks = null;
    }
    private void OnSpellPerformed(InputAction.CallbackContext context,int slotIndex)
    {
        if (!inputEnabled) return;
        // "이 슬롯을 눌렀다"는 의도만 기록
        
        bufferedSpellSlot = slotIndex;
        bufferedSpellTime = Time.unscaledTime;   // hitStop 중에도 안전
        OnSpellInput?.Invoke(slotIndex);
    }
    private void BindSpellInputs()
    {
        spellActions = new[] {
            inputActions.PlayerAction.Spell1,
            inputActions.PlayerAction.Spell2,
            inputActions.PlayerAction.Spell3,
            inputActions.PlayerAction.Spell4
        };
        spellCallbacks = new Action<InputAction.CallbackContext>[spellActions.Length];
        for (int i = 0; i < spellActions.Length; i++)
        {
            int slotIndex = i; // 로컬 변수로 캡처
            spellCallbacks[i] = ctx=> OnSpellPerformed(ctx,slotIndex);
            spellActions[i].performed += spellCallbacks[i];
        }
    }
    /// <summary>상태에서 호출. 유효한 입력이 있으면 슬롯 번호를 반환하고 버퍼를 비움. 없으면 -1.</summary>
    public int PeekSpellInput()
    {
        if (bufferedSpellSlot < 0) return -1;
        if (Time.unscaledTime - bufferedSpellTime > SpellBufferTime)
        {
            bufferedSpellSlot = -1;
            return -1;
        }
        return bufferedSpellSlot;
    }

    public void ClearSpellInput() => bufferedSpellSlot = -1;
    #endregion
    #region UI Input Handling
    private void OnUITogglePerformed(InputAction.CallbackContext context)
    {
        if (uiToggleMap.TryGetValue(context.action, out UIType uiType))
            OnUIToggleInput?.Invoke(uiType);
    }
    #endregion
    #region Camera Input Handling
    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        if (!inputEnabled)
            return;

        Vector2 value = context.ReadValue<Vector2>();

        OnLookInput?.Invoke(value);
    }
    private void OnLockOnPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log("LockOnPerformed");
        OnLockOnClicked?.Invoke();
        //IsLockedOn = !IsLockedOn;
    }
    private void OnLockOnTargetSwitched(InputAction.CallbackContext context)
    {

    }
    #endregion
    private void LateUpdate()
    {
        RollTriggered = false;
        JumpTriggered = false;
        AttackTriggered = false;
        HeavyAttackTriggered = false;
        ParryTriggered = false;
        // WasRunningAtRollTrigger는 RollState.OnEnter()에서 값을 읽어갈 시간이 필요하므로
        // 여기서 강제로 초기화하지 않고, 다음 RollTriggered 발생 시 TriggerRoll()에서 덮어씁니다.
    }
    private void Update()
    {
        // 공유 키 모드에서만 홀드 임계값 판정 수행
        if (IsSharedKeyMode() && isHeld && !hasBecomeRun && (Time.unscaledTime - pressStartTime >= holdThreshold))
        {
            IsRunPressed = true;
            hasBecomeRun = true;
        }
    }
}