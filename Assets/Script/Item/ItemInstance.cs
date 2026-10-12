using System;

public abstract class ItemInstance
{
    public ItemData Data { get; private set; }

    protected ItemInstance(ItemData data) { Data = data; }

    public abstract ItemSaveData ToSave();
}

public class EquipmentInstance : ItemInstance
{
    public string InstanceID { get; private set; }
    public int enhanceLevel;
    public int durability;

    public EquipmentItemData EquipData => (EquipmentItemData)Data;

    // 새로 획득한 장비
    public EquipmentInstance(EquipmentItemData data) : base(data)
    {
        InstanceID = Guid.NewGuid().ToString();
    }

    // 세이브에서 복원
    public EquipmentInstance(EquipmentItemData data, ItemSaveData save) : base(data)
    {
        InstanceID = string.IsNullOrEmpty(save.instanceID)
            ? Guid.NewGuid().ToString()
            : save.instanceID;
        enhanceLevel = save.enhanceLevel;
        durability = save.durability;
    }

    public override ItemSaveData ToSave() => new ItemSaveData
    {
        itemID = Data.itemId,
        instanceID = InstanceID,
        enhanceLevel = enhanceLevel,
        durability = durability
    };
}

// 소모품, 재료 공용 (같은 ID끼리 합쳐서 count로 관리)
public class StackItemInstance : ItemInstance
{
    public int count;

    public StackItemInstance(ItemData data, int count) : base(data)
    {
        this.count = count;
    }

    public override ItemSaveData ToSave() => new ItemSaveData
    {
        itemID = Data.itemId,
        count = count
    };
}