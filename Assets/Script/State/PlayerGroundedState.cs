using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(PlayerController controller) : base(controller)
    {
        this.controller = controller;
    }
    public override void OnEnter()
    {
    }
    public override void Update()
    {
    }
    public override void OnExit()
    {
    }
}
