using UnityEngine;

public class PlayerEmptyActionState : PlayerState   
{
    public PlayerEmptyActionState(Player player, Animator animator) : base(player, animator)
    {
    }
    public override void OnEnter()
    {
    }
    public override void Update()
    {
        if (player.Status.CanAttack && player.Input.AttackTriggered)
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.AttackComboState);
            return;
        }
        else if (player.Status.CanGuard && player.Input.IsGuardHold)
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.GuardState);
            return;
        }
        else if (player.Status.CanParry && player.Input.ParryTriggered&&player.Attack.GetSubWeaponCanParry())
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.ParryState);
            return;
        }
        
    }

    public override void OnExit()
    {
    }
}
