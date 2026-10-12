// EnemyAttack.cs
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private EnemyAttackSet attackSet;

    private EnemyController controller;
    private readonly Dictionary<EnemyAttackPattern, float> cooldownEndTime = new();

    public EnemyAttackPattern CurrentPattern { get; private set; }
    public int CurrentStepIndex { get; private set; }
    public EnemyAttackStepData CurrentStep => CurrentPattern.steps[CurrentStepIndex];
    public bool HasNextStep => CurrentPattern != null && CurrentStepIndex + 1 < CurrentPattern.steps.Count;
    [SerializeField]private Weapon weapon;

    void Awake()
    {
        controller = GetComponent<EnemyController>();
    }

    // ───── 패턴 선택/시작 ─────

    /// <summary>인덱스로 직접 시작 (허수아비처럼 고정 패턴용)</summary>
    public bool TryStartPattern(int index)
    {
        if (attackSet == null || index < 0 || index >= attackSet.patterns.Count) return false;
        return TryStartPattern(attackSet.patterns[index]);
    }

    /// <summary>거리에 맞는 패턴 중 가중치 랜덤으로 시작 (일반 몬스터 AI용)</summary>
    public bool TryStartRandomPattern(float distanceToTarget)
    {
        if (attackSet == null) return false;

        float totalWeight = 0f;
        var candidates = new List<EnemyAttackPattern>();
        foreach (var p in attackSet.patterns)
        {
            if (!IsUsable(p, distanceToTarget)) continue;
            candidates.Add(p);
            totalWeight += p.weight;
        }
        if (candidates.Count == 0) return false;

        float roll = Random.value * totalWeight;
        foreach (var p in candidates)
        {
            roll -= p.weight;
            if (roll <= 0f) return TryStartPattern(p);
        }
        return TryStartPattern(candidates[candidates.Count - 1]);
    }

    public bool TryStartPattern(EnemyAttackPattern pattern)
    {
        if (pattern == null || pattern.steps.Count == 0) return false;
        if (!controller.IsActionIdle) return false;
        if (IsOnCooldown(pattern)) return false;

        CurrentPattern = pattern;
        CurrentStepIndex = 0;
        controller.ActionFSM.ChangeState(controller.AttackState);
        return true;
    }

    bool IsUsable(EnemyAttackPattern p, float distance)
        => p.steps.Count > 0 && p.InRange(distance) && !IsOnCooldown(p);

    bool IsOnCooldown(EnemyAttackPattern p)
        => cooldownEndTime.TryGetValue(p, out float end) && Time.time < end;

    // ───── AttackState가 호출 ─────

    public void AdvanceStep()
    {
        CurrentStepIndex++;
    }

    public void EndPattern()
    {
        if (CurrentPattern != null && CurrentPattern.cooldown > 0f)
            cooldownEndTime[CurrentPattern] = Time.time + CurrentPattern.cooldown;

        CurrentPattern = null;
        CurrentStepIndex = 0;
    }
    public void EnableHitBox()
    {
        if (weapon == null || weapon.WeaponHitBox == null)
        {
            Debug.Log("Weapon.WeaponHitBox==null");
            return;
        }

        // 패턴이 끝난 뒤 늦게 도착한 애니메이션 이벤트 방어
        if (CurrentPattern == null) return;

        // 현재 스텝의 공격 데이터를 주입한 뒤 판정 활성화
        weapon.WeaponHitBox.InitializeAttackData(CurrentStep);
        weapon.WeaponHitBox.EnableHitBox();
    }

    public void DisableHitBox()
    {
        if (weapon == null || weapon.WeaponHitBox == null)
        {
            Debug.Log("Weapon.WeaponHitBox==null");
            return;
        }

        weapon.WeaponHitBox.DisableHitBox();
    }
}