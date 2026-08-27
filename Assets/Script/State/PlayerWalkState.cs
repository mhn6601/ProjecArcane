using UnityEngine;

public class PlayerWalkState : PlayerState
{
    public PlayerWalkState(PlayerController controller, Player player) : base(controller, player)
    {
        this.controller = controller;
        this.player = player;
    }
    public override void OnEnter()
    {
        controller.Animator.SetBool(AnimatorControllerParameterName.isWalk.ToString(), true);
        //Debug.Log("WalkState OnEnter"+ AnimatorControllerParameterName.isWalk.ToString());
    }
    public override void Update()
    {
        // Implement the logic for the Walk state here
    }

    public override void OnExit()
    {
        controller.Animator.SetBool(AnimatorControllerParameterName.isWalk.ToString(), false);
        //Debug.Log("WalkState OnExit"+ AnimatorControllerParameterName.isWalk.ToString());
    }
}
