using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatData statData;

    // --- Properties ---
    public int Level { get; private set; } = 1;
    

    public float MaxHealth => statData.baseMaxHealth + (Level - 1) * statData.healthPerLevel;
    public float MaxMana => statData.baseMaxMana + (Level - 1) * statData.manaPerLevel;
    public float MaxStamina => statData.baseMaxStamina + (Level - 1) * statData.staminaPerLevel;
    public float AttackPower => statData.baseAttackPower + (Level - 1) * statData.attackPerLevel;
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
    public void SetStats(int level)
    {
        Level = level;

    }
    public void SetEquipmentBonus()
    {

    }

}
