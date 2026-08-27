using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerController controller, Player player) : base(controller, player)
    {
        this.controller = controller;
        this.player = player;
    }

    public override void OnEnter()
    {
        controller.Animator.SetBool(AnimatorControllerParameterName.isIdle.ToString(), true);
    }
    public override void Update()
    {
        // Implement the logic for the Idle state here
    }

    public override void OnExit()
    {
        controller.Animator.SetBool(AnimatorControllerParameterName.isIdle.ToString(), false);
    }
}
