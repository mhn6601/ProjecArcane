using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public override void OnEnter()
    {
        // 점프 상태 진입 시 실행될 코드 추가
        controller.Animator.SetBool(AnimatorControllerParameterName.isJump.ToString(), true);
    }

    public override void OnExit()
    {
        // 점프 상태 종료 시 실행될 코드 추가
        controller.Animator.SetBool(AnimatorControllerParameterName.isJump.ToString(),false);
    }
}
