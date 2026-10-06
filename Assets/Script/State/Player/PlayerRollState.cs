using UnityEngine;


public class PlayerRollState : PlayerState
{
    private float stateNormalizedTimeMax = 0.85f; // 구르기 최대시간
    private static readonly int RollSubstate = Animator.StringToHash("Roll");
    private static readonly int BaseRoll = Animator.StringToHash("BaseRoll");
    private static readonly int RunRoll = Animator.StringToHash("RunRoll");
    private float immuneTime;
    private float currentImmuneTime;
    Vector3 lastInput;

    public PlayerRollState(Player player, Animator animator) : base(player, animator)
    {

    }

    public override void OnEnter()
    {
        
        currentImmuneTime = 0f;
        immuneTime = player.Stats.RollImmuneTime;
        if (player.Controller.ActionFSM.CurrentState != player.Controller.EmptyActionState)
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
        }
        player.Status.RemoveFlag(CharacterActionFlags.CanMove);
        player.Status.RemoveFlag(CharacterActionFlags.CanAttack);
        player.Status.RemoveFlag(CharacterActionFlags.CanGuard);
        player.Status.RemoveFlag(CharacterActionFlags.CanJump);
        player.Status.RemoveFlag(CharacterActionFlags.CanParry);
        player.Status.RemoveFlag(CharacterActionFlags.CanRoll);
        player.Status.RemoveFlag(CharacterActionFlags.CanRotate);
        player.Status.RemoveFlag(CharacterActionFlags.CanCastSpell);

        player.Status.AddFlag(CharacterActionFlags.IsInvincible);
        player.Status.AddFlag(CharacterActionFlags.IsSuperArmor);

        ApplyRollDirectionSnap();
       
        if (player.Input.RollWhileRunning)
        {
            animator.CrossFade(RunRoll, 0f);
        }
        else
        {
            animator.CrossFade(BaseRoll, 0f);
        }
    }
    public override void Update()
    {
        player.Roll.ExcuteRoll();
        currentImmuneTime += Time.deltaTime;
        if(currentImmuneTime >= immuneTime)
        {
            player.Status.RemoveFlag(CharacterActionFlags.IsInvincible);
            player.Status.RemoveFlag(CharacterActionFlags.IsSuperArmor);
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 종료 시 대기(EmptyActionState)로 복귀
        if (stateInfo.normalizedTime >= stateNormalizedTimeMax)
        {
            player.Controller.MovementFSM.ChangeState(player.Controller.locomotionState);
        }
        
    }
    public override void OnExit()
    {
        player.Status.AddFlag(CharacterActionFlags.CanMove);
        player.Status.AddFlag(CharacterActionFlags.CanAttack);
        player.Status.AddFlag(CharacterActionFlags.CanGuard);
        player.Status.AddFlag(CharacterActionFlags.CanJump);
        player.Status.AddFlag(CharacterActionFlags.CanParry);
        player.Status.AddFlag(CharacterActionFlags.CanRoll);
        player.Status.AddFlag(CharacterActionFlags.CanRotate);
        player.Status.AddFlag(CharacterActionFlags.CanCastSpell);
    }
    private void ApplyRollDirectionSnap()
    {
        Vector3 rawInput = player.Input.MoveInput;
        Vector3 rollDirWorld = rawInput.sqrMagnitude > 0.001f
            ? player.Move.GetWorldDirection(rawInput)
            : player.transform.forward;

        // 락온 여부 관계없이 입력 방향으로 즉시 회전 → 루트모션이 그 방향으로 곧게 나감
        player.Move.RotateInstant(rollDirWorld);
        player.Roll.SetRollDirection(rollDirWorld);
    }
}
