using UnityEngine;

public enum EquipSlot
{
    Weapon,
    Shield,
    Head,
    Chest,
    Legs,
    Accessory,
}
public class EquipmentItemData : ItemData
{
    [Header("장비 공통 스탯")]
    public EquipSlot equipSlot;
    public int requiredLevel = 0;

    // 인벤토리에서 장착 가능 여부 등을 판단할 때 재사용
    public virtual bool CanEquip(Player player) => true;
}
