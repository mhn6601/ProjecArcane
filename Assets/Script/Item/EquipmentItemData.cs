using UnityEngine;

public enum EquipmentSlotType
{
    MainWeapon,
    SubWeapon,
    Shield,
    Head,
    Chest,
    Legs,
    Accessory,
}
public class EquipmentItemData : ItemData
{
    [Header("장비 공통 스탯")]
    public EquipmentSlotType equipSlottype;
    public int requiredLevel = 0;

    [Header("스탯 보너스")]
    public StatBonus[] statBonuses;   

    // 인벤토리에서 장착 가능 여부 등을 판단할 때 재사용
    public virtual bool CanEquip(Player player) => true;

    public void GetItemByID(string itemID)
    {

    }
}
