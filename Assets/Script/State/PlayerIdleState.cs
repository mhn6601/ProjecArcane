using UnityEngine;

public class PlayerIdleState : PlayerState
{
    
    public override void OnEnter()
    {
        controller.Animator.SetBool(AnimatorControllerParameterName.isIdle.ToString(), true);
    }

    public override void OnExit()
    {
        controller.Animator.SetBool(AnimatorControllerParameterName.isIdle.ToString(), false);
    }
}
