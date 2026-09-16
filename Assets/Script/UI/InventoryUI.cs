using UnityEngine;

public class InventoryUI : UIBase
{
    [SerializeField]
    private InventorySlot[] inventorySlots;

    [SerializeField]
    private EquipmentSlot[] equipmentSlots;
    [SerializeField]
    private GameObject InventoryContent;

    private void Awake()
    {
    }
    private void OnEnable()
    {
        InventoryContent.GetComponentsInChildren<InventorySlot>();

    }
    public override void Open()
    {
        base.Open();
        RefreshUI();
    }

    public void RefreshUI()
    {
        RefreshInventory();
        RefreshEquipment();
    }
    private void RefreshInventory()
    {
        var inventory = InventoryManager.Instance.Inventory;

        // 모든 슬롯을 먼저 초기화
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].Clear();
        }

        // 실제 아이템 표시
        for (int i = 0; i < inventory.Count && i < inventorySlots.Length; i++)
        {
            inventorySlots[i].SetItem(inventory[i]);
        }
    }

    private void RefreshEquipment()
    {
        var equippedItems = InventoryManager.Instance.EquipedItemDictionary;

        // 모든 장비 슬롯 초기화
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            equipmentSlots[i].Clear();
        }

        // 장착된 아이템 표시
        foreach (var equipmentSlot in equipmentSlots)
        {
            EquipmentSlotType slotType = equipmentSlot.ItemType;

            if (equippedItems.TryGetValue(slotType, out EquipmentItemData item))
            {
                equipmentSlot.SetItem(item);
            }
        }
    }

}
