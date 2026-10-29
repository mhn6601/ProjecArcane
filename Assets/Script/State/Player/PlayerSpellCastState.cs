using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpellCastState : PlayerState
{
    private readonly SpellData spell;
    private RuntimeSpellInstance spellInstance;  

    private const int AnimLayer = 0;
    private const float EnterTimeout = 0.5f;      // 이 시간 안에 시전 애니메이션에 진입하지 못하면 강제 종료

    private bool fired;
    private bool animEntered;
    private float enterTime;

    public PlayerSpellCastState(Player player, Animator animator, RuntimeSpellInstance instance)
        : base(player, animator)
    {
        this.spell = instance.baseData;
        this.spellInstance = instance;
    }

    public override void OnEnter()
    {
        fired = false;
        animEntered = false;
        enterTime = Time.time;

        // 발사는 애니메이션 이벤트가 담당
        player.CastSpell.OnSpellFire += HandleSpellFire;

        // 시전 중 이동 차단 등: 플래그 이름은 프로젝트의 CharacterActionFlags에 맞게
        // player.Status.AddFlag(CharacterActionFlags.Casting);
        player.Status.RemoveFlag(CharacterActionFlags.CanMove);   // 이동 차단
        player.Status.RemoveFlag(CharacterActionFlags.CanJump);   // 점프  차단
        player.Status.RemoveFlag(CharacterActionFlags.CanRoll);   // 구르기  차단
        player.Status.RemoveFlag(CharacterActionFlags.CanParry);   // 방어  차단
        player.Status.RemoveFlag(CharacterActionFlags.CanGuard);   // 가드  차단
        player.Status.RemoveFlag(CharacterActionFlags.CanAttack);   // 공격  차단


        animator.CrossFade(spell.animationClip.name, 0f, AnimLayer);
    }

    public override void Update()
    {
        // CrossFade 직후에는 이전 상태의 normalizedTime이 읽히므로, 목표 애니메이션인지 먼저 확인
        AnimatorStateInfo info = animator.IsInTransition(AnimLayer)
            ? animator.GetNextAnimatorStateInfo(AnimLayer)
            : animator.GetCurrentAnimatorStateInfo(AnimLayer);

        if (!info.IsName(spell.animationClip.name))
        {
            // 진입한 적이 있는데 다른 스테이트로 넘어갔다면 종료,
            // 아직 한 번도 진입하지 못했다면(스테이트 이름 오타 등) 타임아웃 후 종료해 상태에 갇히는 것을 방지
            if (animEntered || Time.time - enterTime > EnterTimeout)
            {
                if (!animEntered)
                    Debug.LogWarning($"[PlayerSpellCastState] Animator에서 '{spell.animationClip.name}' 스테이트를 찾지 못했습니다.");
                ExitToEmpty();
            }
            return;
        }

        animEntered = true;

        // 애니메이션이 끝까지 재생되면 종료
        if (info.normalizedTime >= 1f)
            ExitToEmpty();
    }

    public override void OnExit()
    {
        player.CastSpell.OnSpellFire -= HandleSpellFire;   // 반드시 해제

        // player.Status.RemoveFlag(CharacterActionFlags.Casting);
        player.Status.AddFlag(CharacterActionFlags.CanMove);   // 이동 차단
        player.Status.AddFlag(CharacterActionFlags.CanJump);   // 점프  차단
        player.Status.AddFlag(CharacterActionFlags.CanRoll);   // 구르기  차단
        player.Status.AddFlag(CharacterActionFlags.CanParry);   // 방어  차단
        player.Status.AddFlag(CharacterActionFlags.CanGuard);   // 가드  차단
        player.Status.AddFlag(CharacterActionFlags.CanAttack);   // 공격  차단

        // 발사 전에 중단된 경우: 구독이 해제되므로 이후 이벤트가 와도 발사되지 않음
    }

    private void ExitToEmpty()
    {
        player.Controller.ActionFSM.ChangeState(new PlayerEmptyActionState(player, animator));
    }

    private void HandleSpellFire()
    {
        if (fired) return;   // 중복 이벤트 방어
        fired = true;
        Fire();
    }

    private void Fire()
    {
        MagicSocket socket = player.MagicSocketHandler.GetSocket(MagicSocketType.RightHand);
        Transform spawnPoint = socket != null ? socket.SocketTransform : player.transform;

        SpellCastContext context = new SpellCastContext(
            player.gameObject,
            spawnPoint,
            player.LockOn.AimPoint,
            spellInstance,
            player.Stats.SpellPower
        );
        GameObject spellPrefab = spell.VfxPrefab;
        if (spellPrefab == null)
        {
            Debug.LogWarning($"[PlayerSpellCastState] {spell.spellName}의 VfxPrefab이 null입니다.");
            return;
        }

        int count = Mathf.Max(1, context.SplitCount);
        float damageMul = count > 1 ? context.SplitDamageMultiplier : 1f;

        for (int i = 0; i < count; i++)
        {
            // 3발이면 -step, 0, +step
            float offset = (i - (count - 1) * 0.5f) * context.SplitAngleStep;

            SpellCastContext shot = context.Clone();
            shot.CastDirection = Quaternion.AngleAxis(offset, Vector3.up) * context.CastDirection;
            shot.FinalDamage = SpellDamageCalculator.CalculateBase(player, spell);
            shot.SplitCount = 1;   // 생성된 투사체가 다시 분열하지 않도록
            shot.UseCastDirectionRotation = count > 1;

            player.MagicSocketHandler.CastSpell(spellPrefab, MagicSocketType.RightHand, shot);
        }
    }
}