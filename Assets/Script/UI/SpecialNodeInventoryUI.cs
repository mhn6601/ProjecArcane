using System.Collections.Generic;
using UnityEngine;

public class SpecialNodeInventoryUI : UIBase
{
    [Header("Special Node Inventory")]
    private List<SpecialNodeInventorySlot> SpecialNodeInventorySlots =
        new List<SpecialNodeInventorySlot>();

    public int SlotCount
    {
        get
        {
            SpecialNodeInventorySlots.RemoveAll(s => s == null);
            return SpecialNodeInventorySlots.Count;
        }
    }

    [SerializeField]
    private GameObject SpecialNodeInventoryContent;


    private void OnEnable()
    {
        RefreshSlotList();
        if (SpecialNodeInventoryManager.Instance != null)
        {
            SpecialNodeInventoryManager.Instance.specialNodeInventoryChanged
                += RefreshUI;
        }

        // UI가 활성화되었을 때 현재 인벤토리 상태를 즉시 반영
        RefreshUI();
    }
    private void OnDisable()
    {
        if (SpecialNodeInventoryManager.Instance != null)
        {
            SpecialNodeInventoryManager.Instance.specialNodeInventoryChanged
                -= RefreshUI;
        }
    }

    public override void Open()
    {
        base.Open();

        RefreshSlotList();
        RefreshUI();
    }


    /// <summary>
    /// Content 아래에 존재하는 슬롯들을 리스트에 등록합니다.
    /// </summary>
    private void RefreshSlotList()
    {
        if (SpecialNodeInventoryContent == null)
        {

            return;
        }

        SpecialNodeInventorySlots.Clear();

        SpecialNodeInventorySlot[] slots =
            SpecialNodeInventoryContent
                .GetComponentsInChildren<SpecialNodeInventorySlot>(true);

        foreach (SpecialNodeInventorySlot slot in slots)
        {
            if (slot != null)
            {
                SpecialNodeInventorySlots.Add(slot);
            }
        }

    }


    /// <summary>
    /// Manager에서 동적으로 생성한 슬롯을 등록합니다.
    /// </summary>
    public void SetSpecialNodeInventorySlots(
        SpecialNodeInventorySlot slot)
    {
        if (slot == null)
        {

            return;
        }

        if (SpecialNodeInventorySlots.Contains(slot))
        {
            return;
        }

        SpecialNodeInventorySlots.Add(slot);

    }


    public void RefreshUI()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        RefreshInventory();
    }


    private void RefreshInventory()
    {
        if (SpecialNodeInventoryManager.Instance == null)
        {
            return;
        }

        var inventory =
            SpecialNodeInventoryManager.Instance.SpecialNodeInventory;



        // 모든 슬롯 초기화
        for (int i = 0;
             i < SpecialNodeInventorySlots.Count;
             i++)
        {
            if (SpecialNodeInventorySlots[i] == null)
                continue;

            SpecialNodeInventorySlots[i].Clear();
        }


        // 인벤토리 아이템 표시
        for (int i = 0;
             i < inventory.Count &&
             i < SpecialNodeInventorySlots.Count;
             i++)
        {
            if (SpecialNodeInventorySlots[i] == null)
                continue;

            SpecialNodeInventorySlots[i].SetItem(inventory[i]);
        }
    }
}