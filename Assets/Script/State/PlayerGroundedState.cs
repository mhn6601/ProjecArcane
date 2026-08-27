using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(PlayerController controller, Player player) : base(controller, player)
    {
        this.controller = controller;
        this.player = player;
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
