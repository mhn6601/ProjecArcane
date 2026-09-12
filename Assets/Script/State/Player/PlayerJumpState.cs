using UnityEngine;

public class PlayerJumpState : PlayerState
{
    private static readonly int JumpSpeedHash = Animator.StringToHash("JumpSpeed");
    private static readonly int IdleJumpHash = Animator.StringToHash("IdleJump");
    private static readonly int WalkJumpHash = Animator.StringToHash("WalkJump");
    private static readonly int RunJumpHash = Animator.StringToHash("RunJump");

    public PlayerJumpState(Player player) : base(player) { }

    public override void OnEnter()
    {
        if (player.Controller.ActionFSM.CurrentState == player.Controller.GuardState)
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
        }

        // 1. 점프 물리 속도 부여 및 점프 가능 플래그 제거
        player.Jump.SetJumpVelocity();
        player.Status.RemoveFlag(CharacterActionFlags.CanRotate);
        player.Status.RemoveFlag(CharacterActionFlags.CanRoll);
        player.Status.RemoveFlag(CharacterActionFlags.CanJump);
        player.Status.RemoveFlag(CharacterActionFlags.CanParry);
        player.Status.RemoveFlag(CharacterActionFlags.CanGuard);

        // 2. 점프 직전 지상 관성 데이터 계산 및 캐싱
        Vector3 lastInput = player.Input.MoveInput;
        bool initialIsRun = player.Input.IsRunPressed;
        float initialSpeedMultiplier = 0f;
        Vector3 initialJumpDirection = Vector3.zero;

        if (lastInput.sqrMagnitude > 0.001f)
        {
            float inputMagnitude = lastInput.magnitude;

            if (player.Controller.CurrentDevice == PlayerController.ControlDeviceType.Gamepad)
            {
                float maxCap = initialIsRun ? 1.0f : 0.5f;
                initialSpeedMultiplier = Mathf.Clamp(inputMagnitude, 0f, maxCap);
            }
            else
            {
                initialSpeedMultiplier = initialIsRun ? 1.0f : 0.5f;
            }

            initialJumpDirection = player.Jump.CalculateDirection(lastInput);
        }

        // 3. 캐싱된 데이터를 AirborneState에 전달
        player.Controller.AirborneState.SetInertiaData(initialJumpDirection, initialIsRun);

        // 4. Sub-State Machine 진입 파라미터 및 애니메이션 재생
        player.Animator.SetFloat(JumpSpeedHash, initialSpeedMultiplier);

        int targetAnimHash = initialSpeedMultiplier == 0
            ? IdleJumpHash
            : (initialSpeedMultiplier >= 0.5f ? WalkJumpHash : RunJumpHash);

        player.Animator.CrossFade(targetAnimHash, 0.1f);
    }

    public override void Update()
    {
        // 점프 도약 처리가 완료되면 공중 제어를 위해 즉시 AirborneState로 전환
        player.Controller.MovementFSM.ChangeState(player.Controller.AirborneState);
    }

    public override void OnExit()
    {
        // 도약 상태 종료 (필요 시 로깅/정리)
    }
}