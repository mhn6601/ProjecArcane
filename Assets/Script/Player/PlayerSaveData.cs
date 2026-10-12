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

    public List<ItemSaveData> items = new List<ItemSaveData>();
    public List<EquippedSlotSaveData> equipped = new List<EquippedSlotSaveData>(); // 장비 instanceID만 참조
}

[Serializable]
public class ItemSaveData
{
    public string instanceID;   // Guid.NewGuid().ToString(), 개별 구분용
    public string itemID;       // 정의(ScriptableObject) 참조
    public int count=1;          // 스택 수량
    public int enhanceLevel;    // 강화 등 인스턴스 상태는 여기에 추가
    public int durability;
}

[Serializable]
public class EquippedSlotSaveData
{
    public EquipmentSlotType slot;
    public string instanceID;   // 아이템 본체는 복사하지 않고 참조만
}