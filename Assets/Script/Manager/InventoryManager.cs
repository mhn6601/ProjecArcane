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

    // ItemInstance는 추상 클래스라 인스펙터 직렬화 대상이 아님
    private List<ItemInstance> inventory = new List<ItemInstance>();

    public IReadOnlyList<ItemInstance> Inventory => inventory;


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

    private Dictionary<EquipmentSlotType, EquipmentInstance> equippedItemDictionary
        = new Dictionary<EquipmentSlotType, EquipmentInstance>();

    public IReadOnlyDictionary<EquipmentSlotType, EquipmentInstance> EquippedItems
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

    /// <summary>
    /// 정의(ItemData)로 아이템 추가.
    /// 장비는 새 인스턴스를 만들고(1개), 소모품/재료는 기존 스택에 합친다.
    /// </summary>
    public bool AddItem(ItemData itemData, int count = 1)
    {
        if (itemData == null || count <= 0)
            return false;

        // 장비는 항상 개별 인스턴스 1개
        if (itemData is EquipmentItemData equipData)
        {
            if (inventory.Count >= inventorySize)
                return false;

            inventory.Add(new EquipmentInstance(equipData));
            inventoryChanged?.Invoke();
            return true;
        }

        int maxStack = GetMaxStack(itemData);

        // 1) 기존 스택에 들어갈 수 있는 여유 계산
        int room = 0;
        foreach (ItemInstance item in inventory)
        {
            if (item is StackItemInstance s && s.Data.itemId == itemData.itemId)
                room += Mathf.Max(0, maxStack - s.count);
        }

        // 2) 남는 수량을 담을 새 슬롯 수 계산 후, 공간 부족이면 전부 취소
        int remaining = Mathf.Max(0, count - room);
        int newSlots = (remaining + maxStack - 1) / maxStack;

        if (inventory.Count + newSlots > inventorySize)
            return false;

        // 3) 기존 스택부터 채우고
        remaining = count;
        foreach (ItemInstance item in inventory)
        {
            if (remaining <= 0) break;

            if (item is StackItemInstance s && s.Data.itemId == itemData.itemId)
            {
                int add = Mathf.Min(remaining, maxStack - s.count);
                if (add <= 0) continue;

                s.count += add;
                remaining -= add;
            }
        }

        // 4) 남은 수량은 새 스택으로
        while (remaining > 0)
        {
            int take = Mathf.Min(remaining, maxStack);
            inventory.Add(new StackItemInstance(itemData, take));
            remaining -= take;
        }

        inventoryChanged?.Invoke();
        return true;
    }

    private static int GetMaxStack(ItemData data)
    {
        return data.isStackable ? Mathf.Max(1, data.maxStackSize) : 1;
    }


    // =========================================================
    // Equip
    // =========================================================

    public bool EquipItem(EquipmentInstance item)
    {
        if (item == null)
            return false;

        // 인벤토리에 있는 장비인지 확인
        if (!inventory.Contains(item))
            return false;

        EquipmentSlotType slotType = item.EquipData.equipSlottype;

        // 새 장비를 먼저 빼므로, 기존 장비를 돌려놓을 자리는 항상 있다.
        inventory.Remove(item);

        if (equippedItemDictionary.TryGetValue(slotType, out EquipmentInstance oldItem)
            && oldItem != null)
        {
            inventory.Add(oldItem);
        }

        equippedItemDictionary[slotType] = item;

        // 실제 플레이어에게 장비 적용
        if (Player.Instance != null &&
            Player.Instance.EquipmentManager != null)
        {
            Player.Instance.EquipmentManager.ApplyEquipment(item.EquipData);
        }

        inventoryChanged?.Invoke();

        return true;
    }


    // =========================================================
    // Unequip
    // =========================================================

    public bool UnEquipItem(EquipmentInstance item)
    {
        if (item == null)
            return false;

        EquipmentSlotType slotType = item.EquipData.equipSlottype;

        if (!equippedItemDictionary.TryGetValue(slotType, out EquipmentInstance equippedItem))
            return false;

        // 현재 장착된 장비가 요청한 장비와 다른 경우
        if (equippedItem != item)
            return false;

        // 인벤토리가 가득 찼는지 확인
        if (inventory.Count >= inventorySize)
            return false;

        equippedItemDictionary.Remove(slotType);
        inventory.Add(item);

        // 실제 플레이어에서 장비 제거
        if (Player.Instance != null &&
            Player.Instance.EquipmentManager != null)
        {
            Player.Instance.EquipmentManager.RemoveEquipment(item.EquipData);
        }

        inventoryChanged?.Invoke();

        return true;
    }
    public bool UnEquipItem(EquipmentItemData data)
    {
        if (data == null)
            return false;

        EquipmentInstance equipped = GetEquippedItem(data.equipSlottype);

        if (equipped == null || equipped.Data != data)
            return false;

        return UnEquipItem(equipped);
    }

    // =========================================================
    // Get Equipped Item
    // =========================================================

    public EquipmentInstance GetEquippedItem(EquipmentSlotType slotType)
    {
        if (equippedItemDictionary.TryGetValue(slotType, out EquipmentInstance item))
        {
            return item;
        }

        return null;
    }


    // =========================================================
    // Save
    // =========================================================

    /// <summary>
    /// 인벤토리 + 장착 중인 장비를 모두 한 목록으로 저장한다.
    /// </summary>
    public List<ItemSaveData> GetItemSaveData()
    {
        List<ItemSaveData> list = new List<ItemSaveData>();

        foreach (ItemInstance item in inventory)
        {
            if (item != null)
                list.Add(item.ToSave());
        }

        foreach (var pair in equippedItemDictionary)
        {
            if (pair.Value != null)
                list.Add(pair.Value.ToSave());
        }

        return list;
    }


    /// <summary>
    /// 슬롯 - 장비 instanceID 매핑만 저장한다.
    /// </summary>
    public List<EquippedSlotSaveData> GetEquippedSlotSaveData()
    {
        List<EquippedSlotSaveData> list = new List<EquippedSlotSaveData>();

        foreach (var pair in equippedItemDictionary)
        {
            if (pair.Value == null)
                continue;

            list.Add(new EquippedSlotSaveData
            {
                slot = pair.Key,
                instanceID = pair.Value.InstanceID
            });
        }

        return list;
    }


    // =========================================================
    // Load (인벤토리 + 장착을 한 번에 복원)
    // =========================================================

    public void LoadInventory(PlayerSaveData saveData)
    {
        if (saveData == null)
        {
            Debug.Log("Inventory saveData == null");
            return;
        }

        if (itemDatabase == null)
        {
            Debug.LogError("[Inventory] ItemDatabase가 없습니다.");
            return;
        }

        inventory.Clear();
        equippedItemDictionary.Clear();

        if (saveData.items == null)
        {
            inventoryChanged?.Invoke();
            return;
        }

        // 1) 모든 아이템을 인스턴스로 복원
        List<ItemInstance> restored = new List<ItemInstance>();
        Dictionary<string, EquipmentInstance> equipmentByID
            = new Dictionary<string, EquipmentInstance>();

        foreach (ItemSaveData save in saveData.items)
        {
            CreateInstances(save, restored);
        }

        foreach (ItemInstance instance in restored)
        {
            if (instance is EquipmentInstance equip)
                equipmentByID[equip.InstanceID] = equip;
        }
        // 2) 장착 슬롯 복원
        HashSet<EquipmentInstance> equippedSet = new HashSet<EquipmentInstance>();

        if (saveData.equipped != null)
        {
            foreach (EquippedSlotSaveData slotSave in saveData.equipped)
            {
                if (slotSave == null || string.IsNullOrEmpty(slotSave.instanceID))
                    continue;

                if (!equipmentByID.TryGetValue(slotSave.instanceID, out EquipmentInstance equip))
                {
                    Debug.LogError($"[Inventory] 장착 instanceID를 찾을 수 없음 : {slotSave.instanceID}");
                    continue;
                }

                equippedItemDictionary[slotSave.slot] = equip;
                equippedSet.Add(equip);

                if (Player.Instance != null &&
                    Player.Instance.EquipmentManager != null)
                {
                    Player.Instance.EquipmentManager.ApplyEquipment(equip.EquipData);
                }
            }
        }

        // 3) 장착되지 않은 나머지는 인벤토리로
        foreach (ItemInstance instance in restored)
        {
            if (instance is EquipmentInstance equip && equippedSet.Contains(equip))
                continue;

            inventory.Add(instance);
        }

        inventoryChanged?.Invoke();
    }

    private void CreateInstances(ItemSaveData save, List<ItemInstance> output)
    {
        if (save == null)
            return;

        ItemData data = itemDatabase.GetItemByID(save.itemID);

        // 삭제/개명된 아이템은 건너뛴다
        if (data == null)
        {
            Debug.LogError($"[Inventory] DB not found : {save.itemID}");
            return;
        }

        if (data is EquipmentItemData equipData)
        {
            output.Add(new EquipmentInstance(equipData, save));
            return;
        }

        int maxStack = GetMaxStack(data);
        int remaining = Mathf.Max(1, save.count);

        while (remaining > 0)
        {
            int take = Mathf.Min(remaining, maxStack);
            output.Add(new StackItemInstance(data, take));
            remaining -= take;
        }
    }

}