using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : UIBase
{
    [SerializeField]
    private List<InventorySlot> inventorySlots=new List<InventorySlot>();

    [SerializeField]
    private List<EquipmentSlot> equipmentSlots=new List<EquipmentSlot>();
    [SerializeField]
    private GameObject InventoryContent;

    private void Awake()
    {
    }
    private void Start()
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
    public void SetInventorySlots(InventorySlot slot)
    {
        inventorySlots.Add(slot);
    }
    public void RefreshUI()
    {
       // Debug.Log("RefreshUI");
        if (gameObject.activeInHierarchy)
        {
            //Debug.Log(gameObject.activeInHierarchy);
            RefreshInventory();
            RefreshEquipment();
        }
    }
    private void RefreshInventory()
    {
        var inventory = InventoryManager.Instance.Inventory;

        // 모든 슬롯을 먼저 초기화
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].Clear();
        }

        // 실제 아이템 표시
        for (int i = 0; i < inventory.Count && i < inventorySlots.Count; i++)
        {
            inventorySlots[i].SetItem(inventory[i]);
        }
    }

    private void RefreshEquipment()
    {
        var equippedItems = InventoryManager.Instance.EquippedItems;

        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            equipmentSlots[i].Clear();
        }

        foreach (var equipmentSlot in equipmentSlots)
        {
            EquipmentSlotType slotType = equipmentSlot.ItemType;

            if (equippedItems.TryGetValue(slotType, out EquipmentInstance item))
            {
                // EquipmentSlot은 아직 정의(EquipmentItemData)를 받는 상태로 유지
                equipmentSlot.SetItem(item.EquipData);
            }
        }
    }

}
