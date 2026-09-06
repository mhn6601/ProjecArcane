using UnityEngine;

public class PlayerEmptyActionState : PlayerState   
{
    public PlayerEmptyActionState(PlayerController controller) : base(controller)
    {
    }
    public override void OnEnter()
    {
    }
    public override void Update()
    {
        if (controller.Status.CanAttack && controller.Input.AttackTriggered)
        {
            controller.ActionFSM.ChangeState(controller.AttackComboState);
            return;
        }
        else if (controller.Status.CanGuard && controller.Input.IsGuardHold)
        {
            controller.ActionFSM.ChangeState(controller.GuardState);
            return;
        }
        else if (controller.Status.CanParry && controller.Input.ParryTriggered)
        {
            controller.ActionFSM.ChangeState(controller.ParryState);
            return;
        }
        
    }

    public override void OnExit()
    {
    }
}
