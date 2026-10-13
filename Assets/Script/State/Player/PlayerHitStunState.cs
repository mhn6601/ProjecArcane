using UnityEngine;

public class PlayerHitStunState : PlayerState
{
    private static readonly int HitStunHash = Animator.StringToHash("HitStun");
    private static readonly int HitStunSpeedHash = Animator.StringToHash("HitStunSpeed");
    private const string HitStunClipName = "HitStun"; // 실제 클립 이름과 동일하게

    float hitStunDuration = 0.3f;
    float hitStunTimer = 0f;
    float clipLength = -1f;

    public PlayerHitStunState(Player player, Animator animator) : base(player, animator)
    {
        clipLength = GetClipLength(HitStunClipName);
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
        player.Status.RemoveFlag(CharacterActionFlags.CanCastSpell);

        // 클립 길이 / 경직 시간 = 재생 속도
        float speed = clipLength > 0f ? clipLength / hitStunDuration : 1f;
        animator.SetFloat(HitStunSpeedHash, speed);

        animator.CrossFade(HitStunHash, 0f);
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
        // 다른 상태에 영향 주지 않도록 속도 원복
        animator.SetFloat(HitStunSpeedHash, 1f);

        player.Status.AddFlag(CharacterActionFlags.CanMove);
        player.Status.AddFlag(CharacterActionFlags.CanJump);
        player.Status.AddFlag(CharacterActionFlags.CanParry);
        player.Status.AddFlag(CharacterActionFlags.CanGuard);
        player.Status.AddFlag(CharacterActionFlags.CanAttack);
        player.Status.AddFlag(CharacterActionFlags.CanRotate);
        player.Status.AddFlag(CharacterActionFlags.CanCastSpell);
    }

    private float GetClipLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        //Debug.LogWarning($"[HitStun] Clip '{clipName}' not found");
        return -1f;
    }
}