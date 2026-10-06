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

        if (player.Status.CanCastSpell)
        {
            int slot = player.Input.PeekSpellInput();
            if (slot >= 0)
            {
                player.Input.ClearSpellInput();   // 시도한 입력은 성공/실패와 상관없이 비움
                if (player.CastSpell.TryCast(slot)) return;   // 성공하면 상태가 전환됨
            }
        }

        if (player.Status.CanGuard && player.Input.IsGuardHold)
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.GuardState);
            return;
        }

        if (player.Status.CanParry && player.Input.ParryTriggered && player.Attack.GetSubWeaponCanParry())
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.ParryState);
            return;
        }
    }

    public override void OnExit()
    {
    }
}