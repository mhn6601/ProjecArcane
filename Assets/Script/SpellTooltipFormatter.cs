using UnityEngine;

public static class SpellTooltipFormatter
{
    /// <summary>
    /// 장착 중인 스페셜 노드까지 반영한 툴팁. 장착된 인스턴스가 있을 때 사용.
    /// </summary>
    public static string Format(RuntimeSpellInstance instance, Player player)
    {
        SpellData data = instance.baseData;
        float perShot = SpellDamageCalculator.CalculatePerShot(player, instance, out int shotCount);

        return Replace(data, perShot, shotCount);
    }

    /// <summary>
    /// 인스턴스가 없는 경우(미장착/미해금 노드 정보창). 스페셜 노드 효과는 반영되지 않는다.
    /// </summary>
    public static string Format(SpellData data, PlayerStats stats)
    {
        Player player = Player.Instance;
        float damage = player != null
            ? SpellDamageCalculator.CalculateBase(player, data)
            : data.baseDamage * stats.SpellPower * data.damageMultiplier;   // Player가 없을 때의 최소 대체

        return Replace(data, damage, 1);
    }

    private static string Replace(SpellData data, float perShot, int shotCount)
    {
        return data.spellDescription
            .Replace("{damage}", Mathf.RoundToInt(perShot).ToString())
            .Replace("{shotCount}", shotCount.ToString())
            .Replace("{totalDamage}", Mathf.RoundToInt(perShot * shotCount).ToString())
            .Replace("{mp}", data.baseMpCost.ToString())
            .Replace("{cooldown}", data.cooldown.ToString("0.#"));
    }
    
}
public static class StatTooltipFormatter
{
    public static string Format(StatNodeData data, PlayerStats stats)
    {
        return data.description
            .Replace("{maxHealth}", Mathf.RoundToInt(stats.MaxHealth).ToString())
            .Replace("{maxMana}", Mathf.RoundToInt(stats.MaxMana).ToString())
            .Replace("{maxStamina}", Mathf.RoundToInt(stats.MaxStamina).ToString())
            .Replace("{attackPower}", Mathf.RoundToInt(stats.AttackPower).ToString())
            .Replace("{spellPower}", Mathf.RoundToInt(stats.SpellPower).ToString())
            .Replace("{moveSpeed}", stats.MoveSpeed.ToString("0.##"))
            .Replace("{runSpeedMultiply}", stats.RunSpeedMultiply.ToString("0.##"))
            .Replace("{jumpHeight}", stats.Jumphight.ToString("0.##"))
            .Replace("{jumpStaminaCost}", stats.JumpStaminaCost.ToString("0.##"))
            .Replace("{staminaRegenRate}", stats.staminaRegenRate.ToString("0.##"))
            .Replace("{staminaRegenBoostMul}", stats.staminaRegenBoostMul.ToString("0.##"))
            .Replace("{staminaRegenDelay}", stats.staminaRegenDelay.ToString("0.##"))
            .Replace("{staminaRegenBoostDelay}", stats.staminaRegenBoostDelay.ToString("0.##"))
            .Replace("{staminaRecoveryThreshold}", stats.staminaRecoveryThreshold.ToString("0.##"))
            .Replace("{runStaminaCostPerSec}", stats.RunStaminaCostPerSec.ToString("0.##"))
            .Replace("{rollStaminaCost}", stats.RollStaminaCost.ToString("0.##"))
            .Replace("{rollImmuneTime}", stats.RollImmuneTime.ToString("0.##"));
    }
}