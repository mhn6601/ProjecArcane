using UnityEngine;
using System.Collections.Generic;
using System;

public class InventoryManager : Singleton<InventoryManager>
{
    [Header("Database")]
    [SerializeField]
    private ItemDatabaseSO itemDatabase;

    [Header("Inventory")]
    [SerializeField]
    private int inventorySize = 20;

    [SerializeField]
    private GameObject inventoryPrefab;

    [SerializeField]
    private Transform inventoryTransform;

    [SerializeField]
    private List<ItemData> inventory = new List<ItemData>();

    public IReadOnlyList<ItemData> Inventory => inventory;


    [Header("Default Equipment")]
    [SerializeField]
    private EquipmentItemData startMainWeapon;
    [SerializeField]
    private EquipmentItemData startSubWeapon;


    [Header("UI")]
    [SerializeField]
    private InventoryUI inventoryUI;


    // =========================================================
    // 장착 상태
    // =========================================================

    // [장비 부위 - 장착 중인 장비]
    private Dictionary<EquipmentSlotType, EquipmentItemData> equippedItemDictionary
        = new Dictionary<EquipmentSlotType, EquipmentItemData>();

    public IReadOnlyDictionary<EquipmentSlotType, EquipmentItemData> EquippedItems
        => equippedItemDictionary;


    // =========================================================
    // Event
    // =========================================================

    public event Action inventoryChanged;


    // =========================================================
    // Unity
    // =========================================================

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
        if (inventoryUI != null)
        {
            inventoryChanged -= inventoryUI.RefreshUI;
        }

        base.OnDestroy();
    }


    // =========================================================
    // Initialize
    // =========================================================

    public void Init()
    {
        if (inventoryTransform == null || inventoryPrefab == null || inventoryUI == null)
            return;

        for (int i = inventoryTransform.childCount; i < inventorySize; i++)
        {
            InventorySlot slot =
                Instantiate(inventoryPrefab, inventoryTransform)
                .GetComponent<InventorySlot>();

            inventoryUI.SetInventorySlots(slot);
        }
    }


    // =========================================================
    // New Game
    // =========================================================

    public void NewGame()
    {
        inventory.Clear();
        equippedItemDictionary.Clear();

        GiveDefaultEquipment();

        inventoryChanged?.Invoke();
    }

    public void GiveDefaultEquipment()
    {
        Debug.Log("GiveDefaultEquipment");

        if (startMainWeapon != null)
        {
            AddItem(startMainWeapon);
        }
        if (startSubWeapon != null)
        {
            AddItem(startSubWeapon);
        }
    }


    // =========================================================
    // Inventory
    // =========================================================

    public bool AddItem(ItemData itemData)
    {
        if (itemData == null)
            return false;

        if (inventory.Count >= inventorySize)
            return false;

        inventory.Add(itemData);

        inventoryChanged?.Invoke();

        return true;
    }


    public bool RemoveItem(ItemData itemData)
    {
        if (itemData == null)
            return false;

        if (!inventory.Contains(itemData))
            return false;

        inventory.Remove(itemData);

        inventoryChanged?.Invoke();

        return true;
    }


    // =========================================================
    // Equip
    // =========================================================

    public bool EquipItem(EquipmentItemData itemData)
    {
        if (itemData == null)
            return false;

        // 인벤토리에 있는 장비인지 확인
        if (!inventory.Contains(itemData))
            return false;


        EquipmentSlotType slotType = itemData.equipSlottype;


        // 기존 장비가 있다면 인벤토리로 돌려놓는다.
        if (equippedItemDictionary.TryGetValue(
                slotType,
                out EquipmentItemData oldItem)
            && oldItem != null)
        {
            // 기존 장비를 인벤토리에 넣을 공간이 있어야 한다.
            // 새 장비를 먼저 제거할 것이므로 현재 Count는 1 감소하게 된다.
            inventory.Remove(itemData);

            inventory.Add(oldItem);

            equippedItemDictionary[slotType] = null;
        }
        else
        {
            // 기존 장비가 없다면 새 장비만 제거
            inventory.Remove(itemData);
        }

        // 장착 상태 저장
        equippedItemDictionary[slotType] = itemData;


        // 실제 플레이어에게 장비 적용
        if (Player.Instance != null &&
            Player.Instance.EquipmentManager != null)
        {
            Player.Instance.EquipmentManager.ApplyEquipment(itemData);
        }


        inventoryChanged?.Invoke();

        return true;
    }


    // =========================================================
    // Unequip
    // =========================================================

    public bool UnEquipItem(EquipmentItemData itemData)
    {
        if (itemData == null)
            return false;


        EquipmentSlotType slotType = itemData.equipSlottype;


        // 해당 슬롯에 장착된 장비 확인
        if (!equippedItemDictionary.TryGetValue(
                slotType,
                out EquipmentItemData equippedItem))
        {
            return false;
        }


        // 현재 장착된 장비가 요청한 장비와 다른 경우
        if (equippedItem != itemData)
            return false;


        // 인벤토리가 가득 찼는지 확인
        if (inventory.Count >= inventorySize)
            return false;


        // 장착 상태 제거
        equippedItemDictionary.Remove(slotType);


        // 인벤토리로 이동
        inventory.Add(itemData);


        // 실제 플레이어에서 장비 제거
        if (Player.Instance != null &&
            Player.Instance.EquipmentManager != null)
        {
            Debug.Log(itemData);
            Player.Instance.EquipmentManager.RemoveEquipment(itemData);
        }


        inventoryChanged?.Invoke();

        return true;
    }


    // =========================================================
    // Get Equipped Item
    // =========================================================

    public EquipmentItemData GetEquippedItem(EquipmentSlotType slotType)
    {
        if (equippedItemDictionary.TryGetValue(
                slotType,
                out EquipmentItemData item))
        {
            return item;
        }

        return null;
    }


   


    // =========================================================
    // Save
    // =========================================================

    public List<string> GetInventoryItemIDs()
    {
        List<string> itemIDs = new List<string>();

        foreach (ItemData item in inventory)
        {
            if (item != null)
            {
                itemIDs.Add(item.itemId);
            }
        }

        return itemIDs;
    }


    public List<EquippedItemSaveData> GetEquippedItemSaveData()
    {
        List<EquippedItemSaveData> saveDataList =
            new List<EquippedItemSaveData>();


        foreach (var pair in equippedItemDictionary)
        {
            if (pair.Value == null)
                continue;

            saveDataList.Add(new EquippedItemSaveData
            {
                slot = pair.Key,
                itemID = pair.Value.itemId
            });
        }

        return saveDataList;
    }


    // =========================================================
    // Load Inventory
    // =========================================================

    public void LoadInventory(PlayerSaveData saveData)
    {
        if (saveData == null)
        {
            Debug.Log("Inventory saveData == null");
            return;
        }


        inventory.Clear();


        if (saveData.inventoryItemIDs == null)
            return;


        //Debug.Log(
        //    $"[Inventory] Load count : {saveData.inventoryItemIDs.Count}"
        //);


        foreach (string itemID in saveData.inventoryItemIDs)
        {
           // Debug.Log($"[Inventory] Load try ID : {itemID}");


            ItemData itemData = itemDatabase.GetItemByID(itemID);


            if (itemData != null)
            {
                //Debug.Log(
                //    $"[Inventory] Item Found : {itemData.itemName}"
                //);

                inventory.Add(itemData);
            }
            else
            {
                //Debug.LogError(
                //    $"[Inventory] DB not found : {itemID}"
                //);
            }
        }


        //Debug.Log(
        //    $"[Inventory] Final count : {inventory.Count}"
        //);


        inventoryChanged?.Invoke();
    }


    // =========================================================
    // Load Equipment
    // =========================================================

    public void LoadEquippedItems(
        List<EquippedItemSaveData> savedEquipList)
    {
        if (savedEquipList == null)
            return;

        if (itemDatabase == null)
        {
            Debug.LogError("[Inventory] ItemDatabase가 없습니다.");
            return;
        }


        // 기존 장착 상태 초기화
        equippedItemDictionary.Clear();


        foreach (EquippedItemSaveData savedData in savedEquipList)
        {
            if (savedData == null)
                continue;


            ItemData itemData =
                itemDatabase.GetItemByID(savedData.itemID);


            EquipmentItemData equipmentItem =
                itemData as EquipmentItemData;


            if (equipmentItem == null)
            {
                Debug.LogError(
                    $"[Inventory] Equipment DB not found : {savedData.itemID}"
                );

                continue;
            }


            EquipmentSlotType slotType =
                savedData.slot;


            // 장착 상태 저장
            equippedItemDictionary[slotType] =
                equipmentItem;


            // 실제 플레이어에게 적용
            if (Player.Instance != null &&
                Player.Instance.EquipmentManager != null)
            {
                Player.Instance.EquipmentManager.ApplyEquipment(
                    equipmentItem
                );
            }
        }


        inventoryChanged?.Invoke();
    }


    // =========================================================
    // Database
    // =========================================================

    public ItemDatabaseSO GetItemDatabase()
    {
        return itemDatabase;
    }
}