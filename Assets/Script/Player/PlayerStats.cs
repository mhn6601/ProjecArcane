using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private PlayerStatData statData;

    // --- Properties ---
    public int Level { get; private set; } = 1;
    public int CurrentExp { get; private set; } = 0;
    public int RequiredExp => statData.GetRequiredExpForLevel(Level);

    public float MaxHealth => statData.baseMaxHealth + (Level - 1) * statData.healthPerLevel;
    public float MaxStamina => statData.baseMaxStamina + (Level - 1) * statData.staminaPerLevel;
    public float AttackPower => statData.baseAttackPower + (Level - 1) * statData.attackPerLevel;
    public float MoveSpeed => statData.moveSpeed;
    public float RunSpeed => statData.runSpeed;

    // --- Events (UI 연결용) ---
    public event Action<int, int> OnExpChanged; // (currentExp, requiredExp)
    public event Action<int> OnLevelUp;         // (newLevel)

    public void AddExp(int amount)
    {
        if (amount <= 0) return;

        CurrentExp += amount;

        // 레벨업 조건 충족 시 처리 (연속 레벨업 대응 while문)
        while (CurrentExp >= RequiredExp)
        {
            CurrentExp -= RequiredExp;
            Level++;
            OnLevelUp?.Invoke(Level);
        }

        OnExpChanged?.Invoke(CurrentExp, RequiredExp);
    }
}
