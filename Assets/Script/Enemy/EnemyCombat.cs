using UnityEngine;
using System;

public class EnemyCombat : CharacterCombat, ICombatTarget
{
    [SerializeField] private Enemy enemy;
    public event Action OnDeath;
    public event Action<AttackData> OnHitTaken; // 피격 이펙트/사운드용
    public event Action OnParrySuccess;          // 패링 이펙트/사운드용
    [SerializeField] private EnemyResourceSystem enemyResourceSystem;
   
    public override void ResolveHit(AttackData attackData, Transform attacker, HitPart hitPart)
    {
        // 1. 무적 프레임 (구르기 등)
        if (enemy.Status.IsInvincible)
            return;

        //// 2. 현재 상태가 EnemyyParryState이고 패링 윈도우 안인지 체크
        //if (enemy.Controller.ActionFSM.CurrentState is EnemyParryState parryState
        //    && parryState.IsInParryWindow)
        //{
        //    if (attackData.canBeParried)
        //    {
        //        HandleParrySuccess(parryState, attacker, attackData);
        //    }
        //    else
        //    {
        //        // 패링 불가 공격(적색 판정 등) -> 그냥 피격 처리
        //        ApplyDamage(attackData, hitPart);
        //    }
        //    return;
        //}

        //// 3. 가드 중인지 체크
        //if (enemy.Status.IsGuarding)
        //{
        //    HandleGuardedHit(attackData);
        //    return;
        //}

        // 4. 일반 피격
        ApplyDamage(attackData, hitPart);
    }
    private void ApplyDamage(AttackData attackData, HitPart hitPart)
    {
        float finalDamage = attackData.damage * GetBodyPartMultiplier(hitPart);
        enemyResourceSystem.TakeDamage(finalDamage);
        OnHitTaken?.Invoke(attackData);

        // 히트스턴 처리: 상태머신으로 전이
        if (enemyResourceSystem.CurrentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
        else
        {
            enemy.Controller.ActionFSM.ChangeState(enemy.Controller.HitStunState);
        }
    }
    public void ApplyStagger(float amount)
    {

    }
    public bool IsDead { get { return enemy.isDead; } }
    private float GetBodyPartMultiplier(HitPart hitpart) =>
        hitpart switch
        {
            HitPart.Head => 1.5f,
            HitPart.Body => 1.0f,
            _ => 1.0f
        };
}
