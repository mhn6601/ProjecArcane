using UnityEditor;
using UnityEngine;
using System;

public class PlayerCombat : CharacterCombat,ICombatTarget
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerResourceSystem playerResourceSystem;

    public event Action OnDeath;
    public event Action<AttackData> OnHitTaken; // 피격 이펙트/사운드용
    public event Action OnParrySuccess;          // 패링 이펙트/사운드용
    public bool IsDead { get { return player.isDead; } }

    public override void ResolveHit(AttackData attackData, Transform attacker, BodyPart bodyPart)
    {
        // 1. 무적 프레임 (구르기 등)
        if (player.Status.IsInvincible)
            return;

        // 2. 현재 상태가 PlayerParryState이고 패링 윈도우 안인지 체크
        if (player.Controller.ActionFSM.CurrentState is PlayerParryState parryState
            && parryState.IsInParryWindow)
        {
            if (attackData.canBeParried)
            {
                HandleParrySuccess(parryState, attacker, attackData);
            }
            else
            {
                // 패링 불가 공격(적색 판정 등) -> 그냥 피격 처리
                ApplyDamage(attackData, bodyPart);
            }
            return;
        }

        // 3. 가드 중인지 체크
        if (player.Status.IsGuarding)
        {
            HandleGuardedHit(attackData);
            return;
        }

        // 4. 일반 피격
        ApplyDamage(attackData, bodyPart);
    }

    private void HandleParrySuccess(PlayerParryState parryState, Transform attacker, AttackData attackData)
    {
        parryState.OnParrySuccess();
        OnParrySuccess?.Invoke();

        // 공격자에게 스태거 부여 (몬스터도 동일한 Combat 인터페이스를 구현한다는 전제)
        if (attacker.TryGetComponent<ICombatTarget>(out var attackerCombat))
        {
            //attackerCombat.ApplyStagger(attackData.parryStaggerAmount);//경직부여 메소드
        }
    }

    private void HandleGuardedHit(AttackData attackData)
    {
        float guardDamage =  attackData.guardDamageMultiplier; // 가드 관통 데미지 비율
        playerResourceSystem.TakeDamage(guardDamage);

        // 가드 브레이크 체크 (자세/스태미나 게이지 있다면 여기서 처리)
    }

    private void ApplyDamage(AttackData attackData, BodyPart bodyPart)
    {
        float finalDamage = attackData.damage * GetBodyPartMultiplier(bodyPart);
        playerResourceSystem.TakeDamage(finalDamage);
        OnHitTaken?.Invoke(attackData);

        // 히트스턴 처리: 상태머신으로 전이
        if (playerResourceSystem.CurrentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
        else
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.HitStunState);
        }
    }

    public void ApplyStagger(float amount)
    {
        // 패링 당했을 때 (플레이어가 몬스터에게 패링 성공 시엔 이건 몬스터 쪽 Combat에서 호출됨)
        //player.Controller.ActionFSM.ChangeState(player.Controller.StaggerState);
    }

    private float GetBodyPartMultiplier(BodyPart part) =>
        part switch
        {
            BodyPart.Head => 1.5f,
            BodyPart.Body => 1.0f,
            _ => 1.0f
        };
}