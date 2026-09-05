using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInputAction inputActions;

    // FSM State들이 읽어갈 입력 값들

    // 지속 유지 입력 Hold
    public Vector3 MoveInput { get; private set; }
    public bool IsRunPressed { get; private set; }
    public bool IsGuardHold { get; private set; }



    // 단발성 트리거 입력Triggered
    public bool JumpTriggered { get; private set; }
    public bool AttackTriggered { get; private set; }
    public bool HeavyAttackTriggered { get; private set; }
    public bool ParryTriggered { get; private set; }

    private void Awake()
    {
        inputActions = new PlayerInputAction();
    }

    private void OnEnable()
    {
        inputActions.PlayerAction.Enable();

        inputActions.PlayerAction.Move.performed += OnMovePerformed;
        inputActions.PlayerAction.Move.canceled += OnMoveCanceled;

        inputActions.PlayerAction.Run.performed += OnRunPerformed;
        inputActions.PlayerAction.Run.canceled += OnRunCanceled;

        inputActions.PlayerAction.Jump.performed += OnJumpPerformed;

        inputActions.PlayerAction.Attack.performed += OnAttackPerformed;
        inputActions.PlayerAction.HeavyAttack.performed += OnHeavyAttackPerformed;
        inputActions.PlayerAction.Parry.performed += OnParryPerformed;
        inputActions.PlayerAction.Guard.performed += OnGuardHoldPerformed;
        inputActions.PlayerAction.Guard.canceled += OnGuardHoldCanceled;

    }

    private void OnDisable()
    {
        inputActions.PlayerAction.Move.performed -= OnMovePerformed;
        inputActions.PlayerAction.Move.canceled -= OnMoveCanceled;

        inputActions.PlayerAction.Run.performed -= OnRunPerformed;
        inputActions.PlayerAction.Run.canceled -= OnRunCanceled;

        inputActions.PlayerAction.Jump.performed -= OnJumpPerformed;

        inputActions.PlayerAction.Attack.performed -= OnAttackPerformed;
        inputActions.PlayerAction.HeavyAttack.performed -= OnHeavyAttackPerformed;
        inputActions.PlayerAction.Parry.performed -= OnParryPerformed;
        inputActions.PlayerAction.Guard.performed -= OnGuardHoldPerformed;
        inputActions.PlayerAction.Guard.canceled -= OnGuardHoldCanceled;

        inputActions.PlayerAction.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector3>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector3.zero;
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        IsRunPressed = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        IsRunPressed = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        JumpTriggered = true;
    }
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        AttackTriggered = true;
    }
    private void OnHeavyAttackPerformed(InputAction.CallbackContext context)
    {
        HeavyAttackTriggered = true;
    }
    private void OnParryPerformed(InputAction.CallbackContext context)
    {
        ParryTriggered = true;
    }
    private void OnGuardHoldPerformed(InputAction.CallbackContext context)
    {
        IsGuardHold = true;
    }
    private void OnGuardHoldCanceled(InputAction.CallbackContext context)
    {
        IsGuardHold = false;
    }


    // 단발성 트리거(Jump 등)는 프레임 종료 시 리셋
    private void LateUpdate()
    {
        JumpTriggered = false;
        AttackTriggered = false;
        HeavyAttackTriggered = false;
        ParryTriggered = false;
    }
}