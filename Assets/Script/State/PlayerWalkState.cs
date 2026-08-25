using UnityEngine;

public class PlayerWalkState : PlayerState
{

    public override void OnEnter()
    {
        controller.Animator.SetBool(AnimatorControllerParameterName.isWalk.ToString(), true);
        //Debug.Log("WalkState OnEnter"+ AnimatorControllerParameterName.isWalk.ToString());
    }

    public override void OnExit()
    {
        controller.Animator.SetBool(AnimatorControllerParameterName.isWalk.ToString(), false);
        //Debug.Log("WalkState OnExit"+ AnimatorControllerParameterName.isWalk.ToString());
    }
}
