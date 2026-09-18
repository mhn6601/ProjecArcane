using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Overlays;
using System.Linq;
using System;

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
    [SerializeField]
    private List<ItemData> inventory = new List<ItemData>();
    private Dictionary<EquipmentSlotType, EquipmentItemData> equipedItemDictionary=new Dictionary<EquipmentSlotType, EquipmentItemData>();
    public IReadOnlyList<ItemData> Inventory => inventory;
    public Dictionary<EquipmentSlotType, EquipmentItemData> EquipedItemDictionary => equipedItemDictionary;

    [SerializeField] private InventoryUI inventoryUI;

    public event Action inventoryChanged;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }
    private void Start()
    {
        if (inventoryUI != null)
        {
            inventoryChanged += inventoryUI.RefreshUI;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (inventoryUI != null)
        {
            inventoryChanged -= inventoryUI.RefreshUI;
        }
    }
    public void Init()
    {
        for (int i = inventoryTransform.childCount; i < inventorySize; i++)
        {
            inventoryUI.SetInventorySlots(Instantiate(inventoryPrefab, inventoryTransform).GetComponent<InventorySlot>());
        }
        
    }
    public void NewGame()
    {
        inventory.Clear();
        equipedItemDictionary.Clear();

        GiveDefaultEquipment();
    }
    
    public void GiveDefaultEquipment()
    {
        Debug.Log("GiveDefaultEquipment");
        AddItem(defaultWeapon);
    }
    public bool AddItem(ItemData itemdata)
    {
        if (itemdata == null) return false;
        if (inventory.Count >= inventorySize) return false;

        inventory.Add(itemdata);
        inventoryChanged?.Invoke();
        return true;
    }
    public bool RemoveItem(ItemData itemdata)
    {
        if(itemdata == null)
        {
            return false;
        }
        if (!inventory.Contains(itemdata))
        {
            return false;
        }
        inventory.Remove(itemdata);

        inventoryChanged?.Invoke();
        return true;
    }
    public bool EquipItem(EquipmentItemData itemData)
    {
        if (itemData == null) return false;
        if (!inventory.Contains(itemData)) return false;

        EquipmentSlotType slotType = itemData.equipSlottype;
        if (equipedItemDictionary.TryGetValue(slotType, out EquipmentItemData oldItem) && oldItem != null)
        {
            inventory.Add(oldItem);
        }
        equipedItemDictionary[slotType] = itemData;

        inventory.Remove(itemData);
        inventoryChanged?.Invoke();
        return true;
    }
    public bool UnEquipItem(EquipmentItemData itemData)
    {
        if (itemData == null)
            return false;

        if (!equipedItemDictionary.TryGetValue(
                itemData.equipSlottype,
                out EquipmentItemData equippedItem))
        {
            return false;
        }

        if (equippedItem != itemData)
            return false;

        if (inventory.Count >= inventorySize)
            return false;

        equipedItemDictionary[itemData.equipSlottype] = null;

        inventory.Add(itemData);

        inventoryChanged?.Invoke();

        return true;
    }
    public void LoadInventory(PlayerSaveData saveData)
    {
        if (saveData == null)
            return;

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
        inventoryChanged?.Invoke();
    }

    public EquipmentItemDatabaseSO GetEquipmentDB()
    {
        return equipmentItemDatabase;
    }
}                   
