using UnityEngine;
using System;

public class EnemyCombat : CharacterCombat, ICombatTarget
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private EnemyController controller;
    //[SerializeField] private EnemyStatus status;
    public event Action OnDeath;
    public event Action<AttackData> OnHitTaken; // 피격 이펙트/사운드용
    public event Action OnParrySuccess;          // 패링 이펙트/사운드용
    [SerializeField] private EnemyResourceSystem resourceSystem;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }
    private void Start()
    {
        controller = enemy.Controller;
        //status = enemy.Status;
        resourceSystem = enemy.ResourceSystem;
    }
    public override void ResolveHit(AttackData attackData, Transform attacker, HitPart hitPart)
    {
        // 1. 무적 프레임
        if (enemy.Status.IsInvincible)
            return;
        ApplyDamage(attackData, hitPart);
    }
    private void ApplyDamage(AttackData attackData, HitPart hitPart)
    {
        float finalDamage = attackData.damage * GetBodyPartMultiplier(hitPart);
        resourceSystem.TakeDamage(finalDamage);
        OnHitTaken?.Invoke(attackData);

        // 히트스턴 처리: 상태머신으로 전이
        if (resourceSystem.CurrentHealth <= 0f)
        {
            OnDeath?.Invoke();
        }
        else
        {
            controller.ActionFSM.ChangeState(controller.HitStunState);
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
