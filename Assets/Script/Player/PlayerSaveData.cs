using System;
using System.Collections.Generic;

[Serializable]
public class PlayerSaveData
{
    // 스탯 및 자원 데이터
    public int level = 1;
    public float currentHealth = 100f;
    public float currentMana = 100f;
    public float currentStamina = 100f;

    // 장착 중인 아이템 데이터 (슬롯 - 아이템 ID 매핑용 리스트)
    public List<EquippedItemSaveData> equippedItems = new List<EquippedItemSaveData>();

    // 인벤토리 데이터 (아이템 ID 목록)
    public List<string> inventoryItemIDs = new List<string>();
}

// Dictionary 대신 JsonUtility에서 직렬화 가능한 1:1 매핑용 구조체/클래스
[Serializable]
public class EquippedItemSaveData
{
    public EquipmentSlotType slot; // 예: "MainWeapon", "Armor" 등 (EquipmentSlot Enum.ToString())
    public string itemID;   // 예: "EQ_SWORD_01"
}