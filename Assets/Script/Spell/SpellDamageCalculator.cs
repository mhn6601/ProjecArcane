using UnityEngine;
using UnityEngine.Rendering;

public static class SpellDamageCalculator
{
    /// <summary>노드 효과 이전의 기본 데미지. 시전 시 Context 생성자에 넘긴다.</summary>
    public static float CalculateBase(Player player, SpellData spell)
    {
        float weaponSpellDamage = 0f;
        WeaponItemData weapon = player.EquipmentManager != null ? player.EquipmentManager.MainWeapon.WeaponItemData : null;
        if (weapon != null) weaponSpellDamage = weapon.baseSpellDamage;
        Debug.Log($"({weaponSpellDamage} + {player.Stats.SpellPower}) * {spell.damageMultiplier}*{spell.baseDamage}\"");
        return (weaponSpellDamage + player.Stats.SpellPower) * spell.damageMultiplier *spell.baseDamage;
    }

    /// <summary>
    /// 장착된 스페셜 노드까지 반영한 "발당" 데미지 (툴팁/미리보기용).
    /// 실제 시전과 같은 SpellCastContext 경로를 쓰므로 노드 로직이 한 곳에만 존재한다.
    /// </summary>
    public static float CalculatePerShot(Player player, RuntimeSpellInstance instance, out int shotCount)
    {
        var ctx = new SpellCastContext(
            player.gameObject,
            player.transform,
            null,
            instance,
            CalculateBase(player, instance.baseData));

        shotCount = Mathf.Max(1, ctx.SplitCount);
        float damageMul = shotCount > 1 ? ctx.SplitDamageMultiplier : 1f;
        return ctx.FinalDamage * damageMul;
    }
}