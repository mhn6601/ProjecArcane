using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    // 공중 유지 동안 사용할 관성 캐시 데이터
    private Vector3 initialJumpDirection;
    private bool initialIsRun;

    public PlayerAirborneState(PlayerController controller) : base(controller)
    {
        this.controller = controller;
    }

    // JumpState 등 외부에서 관성 데이터를 주입받는 메서드
    public void SetInertiaData(Vector3 direction, bool isRun)
    {
        initialJumpDirection = direction;
        initialIsRun = isRun;
    }

    public override void OnEnter()
    {
        controller.Status.RemoveFlag(CharacterActionFlags.CanMove);
        controller.Status.RemoveFlag(CharacterActionFlags.CanJump);
        controller.Status.RemoveFlag(CharacterActionFlags.CanParry);
        controller.Status.RemoveFlag(CharacterActionFlags.CanGuard);
    }

    public override void Update()
    {
        // 1. 공중 이동 처리 (캐싱된 관성으로 유지 이동)
        controller.PlayerJump.ExecuteJump(initialJumpDirection, initialIsRun);

        // 2. 착지 판정 (하강 중이고 지면에 닿았을 때 Locomotion으로 전환)
        if (controller.PlayerJump.GetJumpVelocity() <= 0f && controller.PlayerJump.IsGround())
        {
            controller.MovementFSM.ChangeState(controller.locomotionState);
        }
    }

    public override void OnExit()
    {
        // 착지 시 상태 및 플래그 복구
        controller.Status.AddFlag(CharacterActionFlags.CanMove);
        controller.Status.AddFlag(CharacterActionFlags.CanJump);
        controller.Status.AddFlag(CharacterActionFlags.CanGuard);
        controller.Status.AddFlag(CharacterActionFlags.CanParry);

        // 관성 데이터 초기화
        initialJumpDirection = Vector3.zero;
        initialIsRun = false;
    }
}