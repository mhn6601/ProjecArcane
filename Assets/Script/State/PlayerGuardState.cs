using UnityEngine;

public class PlayerGuardState : PlayerState
{
    public PlayerGuardState(PlayerController controller) : base(controller)
    {
    }
    public override void OnEnter()
    {
        controller.Animator.CrossFade("GuardUp", 0.1f);
        controller.Status.IsGuarding = true; // Guard 상태 플래그 설정[cite: 2]
    }

    public override void Update()
    {
        // 방패 버튼을 떼면 대기 상태로 복귀
        if (!controller.Input.IsGuardHold)
        {
            controller.ActionFSM.ChangeState(controller.EmptyActionState);
        }
    }

    public override void OnExit()
    {
        controller.Status.IsGuarding = false;
    }
}
