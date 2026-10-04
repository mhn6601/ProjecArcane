using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatData statData;

    // 출처(노드 ID, 장비 ID 등)별 보너스. 같은 출처로 다시 넣으면 덮어쓰므로 중복 적용되지 않음
    private readonly Dictionary<string, StatBonus[]> bonusBySource = new Dictionary<string, StatBonus[]>();

    // --- Properties ---
    public int Level { get; private set; } = 1;

    // 보너스가 적용되는 스탯: (레벨 기본값 + 고정 보너스) * (1 + 퍼센트 보너스)
    public float MaxHealth => ApplyBonus(StatType.MaxHp, statData.baseMaxHealth + (Level - 1) * statData.healthPerLevel);
    public float MaxMana => ApplyBonus(StatType.MaxMp, statData.baseMaxMana + (Level - 1) * statData.manaPerLevel);
    public float MaxStamina => ApplyBonus(StatType.MaxStamina, statData.baseMaxStamina + (Level - 1) * statData.staminaPerLevel);
    public float AttackPower => ApplyBonus(StatType.Attack, statData.baseAttackPower + (Level - 1) * statData.attackPerLevel);
    public float SpellPower => ApplyBonus(StatType.SpellPower, statData.baseSpellPower + (Level - 1) * statData.spellPerLevel);

    // 보너스 대상이 아닌 스탯
    public float MoveSpeed => statData.moveSpeed;
    public float RunSpeedMultiply => statData.runSpeedMultiply;
    public float Jumphight => statData.jumphight;
    public float JumpStaminaCost => statData.jumpStaminaCost;
    public float staminaRegenRate => statData.staminaRegenRate;
    public float staminaRegenBoostMul => statData.staminaRegenBoostMul;
    public float staminaRegenDelay => statData.staminaRegenDelay;
    public float staminaRegenBoostDelay => statData.staminaRegenBoostDelay;
    public float staminaRecoveryThreshold => statData.staminaRecoveryThreshold;
    public float RunStaminaCostPerSec => statData.runStaminaCostPerSec;
    public float RollStaminaCost => statData.rollStaminaCost;
    public float RollImmuneTime => statData.rollImmunetime;

    public event Action OnStatsChanged;

    public void SetStats(int level)
    {
        Level = level;
        OnStatsChanged?.Invoke();
    }

    // -------------------------
    // 보너스 (스탯 노드, 장비 등)
    // -------------------------
    public void SetBonuses(string source, StatBonus[] bonuses)
    {
        if (string.IsNullOrEmpty(source)) return;

        bonusBySource[source] = bonuses ?? Array.Empty<StatBonus>();
        OnStatsChanged?.Invoke();
    }

    public void RemoveBonuses(string source)
    {
        if (bonusBySource.Remove(source))
            OnStatsChanged?.Invoke();
    }

    public void SetEquipmentBonus()
    {
        // 장비 보너스도 SetBonuses("equip:" + 장비ID, bonuses)로 연결하면 됨
    }

    private float ApplyBonus(StatType stat, float baseValue)
    {
        float flat = 0f;
        float percent = 0f;

        foreach (var bonuses in bonusBySource.Values)
        {
            foreach (var bonus in bonuses)
            {
                if (bonus.stat != stat) continue;

                if (bonus.modifierType == ModifierType.Flat) flat += bonus.value;
                else percent += bonus.value;
            }
        }

        return (baseValue + flat) * (1f + percent);
    }
}