using UnityEngine;

public class PlayerHitStunState : PlayerState
{
    float hitStunDuration = 0.5f;
    float hitStunTimer = 0f;
    public PlayerHitStunState(Player player, Animator animator) : base(player, animator)
    {
    }
    public override void OnEnter()
    {
        hitStunTimer = 0f;

        player.Status.RemoveFlag(CharacterActionFlags.CanMove);
        player.Status.RemoveFlag(CharacterActionFlags.CanJump);
        player.Status.RemoveFlag(CharacterActionFlags.CanParry);
        player.Status.RemoveFlag(CharacterActionFlags.CanGuard);
        player.Status.RemoveFlag(CharacterActionFlags.CanAttack);
        player.Status.RemoveFlag(CharacterActionFlags.CanRotate);
        
        animator.CrossFade("HitStun", 0f);
    }
    public override void Update()
    {

        hitStunTimer += Time.deltaTime;
        if (hitStunTimer >= hitStunDuration)
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
        }
    }
    public override void OnExit()
    {
        player.Status.RemoveFlag(CharacterActionFlags.CanMove);
        player.Status.RemoveFlag(CharacterActionFlags.CanJump);
        player.Status.RemoveFlag(CharacterActionFlags.CanParry);
        player.Status.RemoveFlag(CharacterActionFlags.CanGuard);
        player.Status.RemoveFlag(CharacterActionFlags.CanAttack);
        player.Status.RemoveFlag(CharacterActionFlags.CanRotate);
    }

}
