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
    public float baseMaxMana = 100f;
    public float baseMaxStamina = 100f;
    public float baseAttackPower = 10f;
    public float baseSpellPower = 10f;

    [Header("Stat Increase Per Level")]
    public float healthPerLevel = 10f;
    public float manaPerLevel = 10f;
    public float staminaPerLevel = 5f;
    public float attackPerLevel = 2f;
    public float spellPerLevel = 2f;

    [Header("BaseSpeed")]
    public float moveSpeed = 2.5f;
    public float runSpeedMultiply = 1.5f;
    [Header("Jump")]
    public float jumphight =1.5f;
    public float jumpStaminaCost =10f;

    [Header("Stamina Settings")]
    public float staminaRegenRate = 5f; // 초당 회복량
    public float staminaRegenBoostMul = 5f; // 부스트 초당 회복량 배율
    public float staminaRegenDelay = 4f; // 사용후 회복 시작 대기시간
    public float staminaRegenBoostDelay = 8f; // 사용후 회복 가속 대기시간

    [Header("RunStaminaCostPerSec")]
    public float runStaminaCostPerSec = 3f;
    public float staminaRecoveryThreshold = 10f;
    [Header("구르기 ")]
    public float rollStaminaCost = 10f;
    public float rollImmunetime = 0.2f;

    // 특정 레벨의 필요 경험치 계산
    public int GetRequiredExpForLevel(int level)
    {
        return Mathf.RoundToInt(baseRequireExp * Mathf.Pow(expGrowthFactor, level - 1));
    }
}