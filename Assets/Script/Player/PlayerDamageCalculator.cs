using System.Diagnostics;
using UnityEngine;

public static class PlayerDamageCalculator
{
    public static float CalculateMelee(Player player, AttackData step)
    {
        float weaponDamage = 0f;
        var main = player.EquipmentManager.MainWeapon;          // 실제 이름에 맞게 변경
        if (main != null) weaponDamage = main.WeaponItemData.basePhysicalDamage;
       
        return (weaponDamage + player.Stats.AttackPower) * step.damageMultiplier;
    }
}