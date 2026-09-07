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
    public float RunSpeed => statData.runSpeed;


}
