using UnityEngine;

public class PlayerRunState : PlayerState
{
    public PlayerRunState(PlayerController controller, Player player) : base(controller, player)
    {
        this.controller = controller;
        this.player = player;
    }
    public override void OnEnter()
    {
        controller.Animator.SetBool(AnimatorControllerParameterName.isRun.ToString(), true);
        //Debug.Log("RunState OnEnter"+ AnimatorControllerParameterName.isRun.ToString());
    }
    public override void Update()
    {
        // Implement the logic for the Run state here
    }

    public override void OnExit()
    {
        controller.Animator.SetBool(AnimatorControllerParameterName.isRun.ToString(), false);
        //Debug.Log("RunState OnExit"+ AnimatorControllerParameterName.isRun.ToString());
    }
    
}
