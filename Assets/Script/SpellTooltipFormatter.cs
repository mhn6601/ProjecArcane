// SpellTooltipFormatter.cs
using UnityEngine;

public static class SpellTooltipFormatter
{
    public static string Format(SpellData data, PlayerStats stats)
    {
        float damage = data.baseDamage + stats.SpellPower * data.spellPowerRatio;

        return data.spellDescription
            .Replace("{damage}", Mathf.RoundToInt(damage).ToString())
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