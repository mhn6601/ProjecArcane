using UnityEngine;

public class PlayerGuardState : PlayerState
{
    readonly int GuardUpHash = Animator.StringToHash("GuardUp");
    readonly int GuardDownHash = Animator.StringToHash("GuardDown");
    private const int GuardLayerIndex = 1;
    public PlayerGuardState(Player player, Animator animator) : base(player, animator)
    {
    }
    public override void OnEnter()
    {
        animator.SetLayerWeight(GuardLayerIndex, 1f); // Guard Layer 활성화
        animator.CrossFade(GuardUpHash, 0f, GuardLayerIndex);
        player.Status.IsGuarding = true; // Guard 상태 플래그 설정[cite: 2]
    }

    public override void Update()
    {
        // 방패 버튼을 떼거나, 달리기 입력이 들어오면 대기 상태로 복귀
        bool releasedGuardButton = !player.Input.IsGuardHold && player.Status.CanGuard;
        bool runInterrupt = player.Input.IsRunPressed;
        bool jumpInterrupt = player.Input.JumpTriggered;

        if (releasedGuardButton || runInterrupt|| jumpInterrupt)
        {
            animator.CrossFade(GuardDownHash, 0f, GuardLayerIndex);
            player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
        }
    }

    public override void OnExit()
    {
        animator.SetLayerWeight(GuardLayerIndex, 0f); // Guard Layer 비활성화
        player.Status.IsGuarding = false;
    }
}
