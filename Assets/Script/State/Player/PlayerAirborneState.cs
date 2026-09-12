using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    // 공중 유지 동안 사용할 관성 캐시 데이터
    private Vector3 initialJumpDirection;
    private bool initialIsRun;

    public PlayerAirborneState(Player player) : base(player)
    {
    }

    // JumpState 등 외부에서 관성 데이터를 주입받는 메서드
    public void SetInertiaData(Vector3 direction, bool isRun)
    {
        initialJumpDirection = direction;
        initialIsRun = isRun;
    }

    public override void OnEnter()
    {
        if (player.Controller.ActionFSM.CurrentState == player.Controller.GuardState)
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
        }

        player.Status.RemoveFlag(CharacterActionFlags.CanMove);
        player.Status.RemoveFlag(CharacterActionFlags.CanRotate);
        player.Status.RemoveFlag(CharacterActionFlags.CanRoll);
        player.Status.RemoveFlag(CharacterActionFlags.CanJump);
        player.Status.RemoveFlag(CharacterActionFlags.CanGuard);
        player.Status.RemoveFlag(CharacterActionFlags.CanParry);
        player.Status.RemoveFlag(CharacterActionFlags.CanAttack);
    }

    public override void Update()
    {
        // 1. 공중 이동 처리 (캐싱된 관성으로 유지 이동)
        player.Jump.ExecuteJump(initialJumpDirection, initialIsRun);

        // 2. 착지 판정 (하강 중이고 지면에 닿았을 때 Locomotion으로 전환)
        if (player.Jump.GetJumpVelocity() <= 0f && player.Jump.IsGround())
        {
            player.Controller.MovementFSM.ChangeState(player.Controller.locomotionState);
        }
    }

    public override void OnExit()
    {
        // 착지 시 상태 및 플래그 복구
        player.Status.AddFlag(CharacterActionFlags.CanMove);
        player.Status.AddFlag(CharacterActionFlags.CanRotate);
        player.Status.AddFlag(CharacterActionFlags.CanRoll);
        player.Status.AddFlag(CharacterActionFlags.CanJump);
        player.Status.AddFlag(CharacterActionFlags.CanGuard);   
        player.Status.AddFlag(CharacterActionFlags.CanParry);   
        player.Status.AddFlag(CharacterActionFlags.CanAttack);   


        // 관성 데이터 초기화
        initialJumpDirection = Vector3.zero;
        initialIsRun = false;
    }
}