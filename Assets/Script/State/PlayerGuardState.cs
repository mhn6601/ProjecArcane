using UnityEngine;

public class PlayerGuardState : PlayerState
{
    readonly int GuardUpHash = Animator.StringToHash("GuardUp");
    readonly int GuardDownHash = Animator.StringToHash("GuardDown");
    private const int GuardLayerIndex = 1;
    public PlayerGuardState(Player player) : base(player)
    {
    }
    public override void OnEnter()
    {
        player.Animator.SetLayerWeight(GuardLayerIndex, 1f); // Guard Layer 활성화
        player.Animator.CrossFade(GuardUpHash, 0f, GuardLayerIndex);
        player.Status.IsGuarding = true; // Guard 상태 플래그 설정[cite: 2]
    }

    public override void Update()
    {
        // 방패 버튼을 떼면 대기 상태로 복귀
        if (!player.Input.IsGuardHold&&player.Status.CanGuard)
        {
            player.Animator.CrossFade(GuardDownHash, 0f, GuardLayerIndex);
            player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
        }
    }

    public override void OnExit()
    {
        player.Animator.SetLayerWeight(GuardLayerIndex, 0f); // Guard Layer 비활성화
        player.Status.IsGuarding = false;
    }
}
