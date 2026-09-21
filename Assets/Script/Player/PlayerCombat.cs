using UnityEditor;
using UnityEngine;
using System;

public class PlayerCombat : CharacterCombat,ICombatTarget
{

    [SerializeField] private Player player;
    [SerializeField] private PlayerStatus status;
    [SerializeField] private PlayerController controller;
    [SerializeField] private PlayerResourceSystem resourceSystem;

    public event Action OnDeath;
    public event Action<AttackData> OnHitTaken; // 피격 이펙트/사운드용
    public event Action OnParrySuccess;          // 패링 이펙트/사운드용
    public bool IsDead { get { return player.isDead; } }

    private void Awake()
    {
        player = Player.Instance;
    }
    private void Start()
    {
        status = player.Status;
        controller = player.Controller;
        resourceSystem = player.ResourceSystem;

    }
    public override void ResolveHit(AttackData attackData, Transform attacker, HitPart hitPart)
    {
        // 1. 무적 프레임 (구르기 등)
        if (status.IsInvincible)
            return;

        // 2. 현재 상태가 PlayerParryState이고 패링 윈도우 안인지 체크
        if (controller.ActionFSM.CurrentState is PlayerParryState parryState
            && parryState.IsInParryWindow)
        {
            if (attackData.canBeParried)
            {
                HandleParrySuccess(parryState, attacker, attackData);
            }
            else
            {
                // 패링 불가 공격(적색 판정 등) -> 그냥 피격 처리
                ApplyDamage(attackData, hitPart);
            }
            return;
        }

        // 3. 가드 중인지 체크
        if (status.IsGuarding)
        {
            HandleGuardedHit(attackData);
            return;
        }

        // 4. 일반 피격
        ApplyDamage(attackData, hitPart);
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
        resourceSystem.TakeDamage(guardDamage);

        // 가드 브레이크 체크 (자세/스태미나 게이지 있다면 여기서 처리)
    }

    private void ApplyDamage(AttackData attackData, HitPart hitPart)
    {
        Debug.Log(attackData);
        Debug.Log(hitPart);
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
        // 패링 당했을 때 (플레이어가 몬스터에게 패링 성공 시엔 이건 몬스터 쪽 Combat에서 호출됨)
        //player.Controller.ActionFSM.ChangeState(player.Controller.StaggerState);
    }

    private float GetBodyPartMultiplier(HitPart hitpart) =>
        hitpart switch
        {
            HitPart.Head => 1.5f,
            HitPart.Body => 1.0f,
            _ => 1.0f
        };
}