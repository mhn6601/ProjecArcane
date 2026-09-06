using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatData", menuName = "Stats/Player Stat Data")]
public class PlayerStatData : ScriptableObject
{
    [Header("Level & Exp Settings")]
    public int baseRequireExp = 100;     // 1레벨 경험치 요구량
    public float expGrowthFactor = 1.2f; // 레벨당 경험치 증가율

    [Header("Base Stats")]
    public float baseMaxHealth = 100f;
    public float baseMaxStamina = 100f;
    public float baseAttackPower = 10f;

    [Header("Stat Increase Per Level")]
    public float healthPerLevel = 10f;
    public float staminaPerLevel = 5f;
    public float attackPerLevel = 2f;

    [Header("BaseSpeed")]
    public float moveSpeed = 2.5f;
    public float runSpeed = 5f;

    // 특정 레벨의 필요 경험치 계산
    public int GetRequiredExpForLevel(int level)
    {
        return Mathf.RoundToInt(baseRequireExp * Mathf.Pow(expGrowthFactor, level - 1));
    }
}