using UnityEngine;

public class EnemyHitStunState : EnemyState
{
    private static readonly int HitStunHash = Animator.StringToHash("HitStun");
    private static readonly int HitStunSpeedHash = Animator.StringToHash("HitStunSpeed");
    private const string HitStunClipName = "HitStun"; // 실제 애니메이션 클립 에셋 이름과 동일하게

    private float hitStunDuration = 0.3f;
    private float hitStunTimer = 0f;
    private float clipLength = -1f;

    public EnemyHitStunState(Enemy enemy, Animator animator) : base(enemy, animator)
    {
        clipLength = GetClipLength(HitStunClipName);
    }

    public override void OnEnter()
    {
        hitStunTimer = 0f;

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
            enemy.Controller.ActionFSM.ChangeState(enemy.Controller.EmptyActionState);
        }
    }

    public override void OnExit()
    {
        // 다른 상태에 영향 주지 않도록 속도 원복
        animator.SetFloat(HitStunSpeedHash, 1f);
    }

    private float GetClipLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        Debug.LogWarning($"[Enemy HitStun] Clip '{clipName}' not found");
        return -1f;
    }
}