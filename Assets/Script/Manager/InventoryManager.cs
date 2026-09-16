using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Overlays;
using System.Linq;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] 
    private EquipmentItemDatabaseSO equipmentItemDatabase;
    [SerializeField]
    private ItemDatabaseSO itemDatabase;
    [SerializeField]
    private int inventorySize = 20;

    [SerializeField]
    private GameObject inventoryPrefab;
    [SerializeField]
    private Transform inventoryTransform;
    [SerializeField]
    private EquipmentItemData defaultWeapon;
    private List<ItemData> inventory = new List<ItemData>();
    private Dictionary<EquipmentSlotType, EquipmentItemData> equipedItemDictionary=new Dictionary<EquipmentSlotType, EquipmentItemData>();
    public IReadOnlyList<ItemData> Inventory => inventory;
    public Dictionary<EquipmentSlotType, EquipmentItemData> EquipedItemDictionary => equipedItemDictionary;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    public void Init()
    {
        for (int i = inventoryTransform.childCount; i < inventorySize; i++)
        {
            Instantiate(inventoryPrefab, inventoryTransform);
        }
        
    }
    public void NewGame()
    {
        inventory.Clear();
        equipedItemDictionary.Clear();

        GiveDefaultEquipment();
    }
    public void EquipDefaultItem(EquipmentItemData item)
    {
        if (item == null)
            return;

        equipedItemDictionary[item.equipSlottype] = item;
    }
    public void GiveDefaultEquipment()
    {
        EquipDefaultItem(defaultWeapon);
    }
    public bool AddItem(ItemData itemdata)
    {
        if (itemdata == null) return false;
        if (inventory.Count >= inventorySize) return false;

        inventory.Add(itemdata);
        return true;
    }
    public bool RemoveItem()
    {
        return true;
    }
    public bool EquipItem(EquipmentItemData itemData)
    {
        if (itemData == null) return false;
        if (!inventory.Contains(itemData)) return false;

        EquipmentSlotType slotType = itemData.equipSlottype;
        if(equipedItemDictionary.TryGetValue(slotType, out EquipmentItemData oldItem))
        {
            inventory.Add(oldItem);
            equipedItemDictionary[slotType] = itemData;

        }
        else
        {
            equipedItemDictionary.Add(slotType, itemData);
        }
        inventory.Remove(itemData);
        return true;
    }
    public bool UnEquipItem(EquipmentItemData itemData)
    {
        if (itemData == null) return false;
        if (!equipedItemDictionary.ContainsKey(itemData.equipSlottype)) return false;

        equipedItemDictionary[itemData.equipSlottype] = null;
        inventory.Add(itemData);
        return true;
    }
    public void LoadInventory(PlayerSaveData saveData)
    {
        inventory.Clear();
        equipedItemDictionary.Clear();

        foreach(string itemId in saveData.inventoryItemIDs)
        {
            ItemData itemData = itemDatabase.GetItemByID(itemId);
            if (itemData != null)
            {
                inventory.Add(itemData);
            }
        }
        foreach (EquippedItemSaveData equipmentSaveData in saveData.equippedItems)
        {
            EquipmentItemData itemData = equipmentItemDatabase.GetItemByID(equipmentSaveData.itemID);
            if (itemData != null)
            {
                EquipedItemDictionary[equipmentSaveData.slot] = itemData;
            }
        }
    }

    public EquipmentItemDatabaseSO GetEquipmentDB()
    {
        return equipmentItemDatabase;
    }
}                   
