using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpecialNodeInventoryManager : Singleton<SpecialNodeInventoryManager>
{
    [Header("Database")]
    [SerializeField]
    private SpecialNodeDatabaseSO specialNodeDatabase;
    [Header("Special Node Inventory")]
    [SerializeField]
    private int specialNodeInventorySize = 0; // 인벤토리 크기
    [SerializeField]
    private GameObject specialNodeInventoryPrefab;
    [SerializeField]
    private Transform specialNodeInventoryTransform;
    private List<SpecialNodeInstance> specialNodeInventory = new List<SpecialNodeInstance>();
    public IReadOnlyList<SpecialNodeInstance> SpecialNodeInventory => specialNodeInventory;
    [Header("UI")]
    [SerializeField]
    private SpecialNodeInventoryUI specialNodeInventoryUI;


    public event Action specialNodeInventoryChanged;
    public event Action<SpecialNodeInstance> NodeSelected;
    protected override void Awake()
    {
        base.Awake();

        Init();
    }
    // =========================================================
    // Initialize
    // =========================================================

    public void Init()
    {
        EnsureSlots(specialNodeInventorySize);
    }
    private void EnsureSlots(int required)
    {
      

        if (specialNodeInventoryTransform == null ||
            specialNodeInventoryPrefab == null ||
            specialNodeInventoryUI == null)
        {
            return;
        }
        while (specialNodeInventoryUI.SlotCount < required)
        {

            SpecialNodeInventorySlot slot =
                Instantiate(
                    specialNodeInventoryPrefab,
                    specialNodeInventoryTransform
                ).GetComponent<SpecialNodeInventorySlot>();


            if (slot == null)
            {

                return;
            }

            specialNodeInventoryUI.SetSpecialNodeInventorySlots(slot);

        }

    }
    public void ShowInfo(SpecialNodeInstance instance)   // 기존 빈 ShowInfo를 교체
    {
        if (instance != null) NodeSelected?.Invoke(instance);
    }
    public bool HasNodeID(string nodeId) =>
    specialNodeInventory.Exists(n => n != null && n.Data != null && n.Data.nodeId == nodeId);

    public SpecialNodeData GetNodeData(string nodeId) =>
        specialNodeDatabase != null ? specialNodeDatabase.GetSpecialNodeByID(nodeId) : null;
    public bool AddSpecialNode(SpecialNodeData specialNodeData)
    {
        if (specialNodeData == null)
        {
            return false;

        }
        // 이미 같은 데이터의 노드가 있으면 추가 취소
        if (specialNodeInventory.Exists(n => n.Data == specialNodeData))
        {
            Debug.Log(specialNodeData.description);
            return false;
        }
        specialNodeInventory.Add(new SpecialNodeInstance(specialNodeData));
        specialNodeInventorySize = Mathf.Max(specialNodeInventorySize, specialNodeInventory.Count);
        EnsureSlots(specialNodeInventorySize);
        specialNodeInventoryChanged?.Invoke();
        return true;
    }
    public void AddAllSpecialNodes()
    {
        var list = specialNodeDatabase.GetNodeList();
        foreach (var item in list)
        {
            bool added = AddSpecialNode(item);
            }
    }
    public List<SpecialNodeSaveData> GetSpecialNodeSaveData()
    {
        List<SpecialNodeSaveData> list = new List<SpecialNodeSaveData>();

        foreach (SpecialNodeInstance node in specialNodeInventory)
        {
            if (node == null) continue;
            var save = node.ToSave();
            list.Add(save);
        }

        return list;
    }
    // =========================================================
    // Load (인벤토리 + 장착을 한 번에 복원)
    // =========================================================

    public void LoadSpecialNodeInventory(PlayerSaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        specialNodeInventory.Clear();
        // equippedItemDictionary.Clear();

        if (saveData.specialNodes == null)
        {
            specialNodeInventoryChanged?.Invoke();
            return;
        }

        specialNodeInventorySize = saveData.specialNodeInventorySize;
        foreach (SpecialNodeSaveData save in saveData.specialNodes)
        {
            CreateInstances(save, specialNodeInventory);
        }
        specialNodeInventorySize = Mathf.Max(specialNodeInventorySize, specialNodeInventory.Count);
        EnsureSlots(specialNodeInventorySize);
        specialNodeInventoryChanged?.Invoke(); 
        if (SpellProgressManager.Instance != null)
            SpellProgressManager.Instance.ValidateSpecialEquips();



    }

    private void CreateInstances(SpecialNodeSaveData save, List<SpecialNodeInstance> output)
    {
        if (save == null)
            return;

        SpecialNodeData data = specialNodeDatabase.GetSpecialNodeByID(save.nodeID);

        if (data == null)
        {
            return;
        }
            output.Add(new SpecialNodeInstance(data, save));
            
    }
    public int GetSpecialNodeInventorySize()
    {
        return specialNodeInventorySize;
    }
}
