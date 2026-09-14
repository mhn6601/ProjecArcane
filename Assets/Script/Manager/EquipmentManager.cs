using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;


public class EquipmentManager : MonoBehaviour
{
    // [부위 - 장착 중인 ItemSO] 딕셔너리 관리
    private Dictionary<EquipmentSlot, EquipmentItemData> equippedItems = new Dictionary<EquipmentSlot, EquipmentItemData>();

    // 무기가 생성되어 손에 쥐어질 위치(소켓) 참조
    [Header("Weapon Sockets")]
    [SerializeField] private Transform mainHandSocket;
    [SerializeField] private Transform subHandSocket;

    // 현재 장착되어 씬에 생성된 GameObject 관리
    private GameObject currentMainWeaponGO;

    // 장비 변경 시 UI 및 스탯 업데이트용 이벤트
    public event Action<EquipmentSlot, EquipmentItemData> OnEquipmentChanged;
    public event Action<EquipmentSlot, WeaponItemData> OnWeaponChanged;

    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }
    /// <summary> 장비 장착 처리 </summary>
    public void Equip(EquipmentItemData newItem)
    {
        if (newItem == null) return;

        EquipmentSlot slot = newItem.equipSlot;
        // 기존에 장착된 동일 부위 아이템이 있다면 해제
        if (equippedItems.ContainsKey(slot) && equippedItems[slot] != null)
        {
            Unequip(slot);
        }

        if (playerStats != null)
        {
            playerStats.SetEquipmentBonus();
        }
        //이벤트 발행(UI 업데이트 등)
        OnEquipmentChanged?.Invoke(slot, newItem);
    }
    public void Equip(WeaponItemData newItem)
    {
        Debug.Log(newItem.itemName);
        if (newItem == null) return;

        EquipmentSlot slot = newItem.equipSlot;

        // 기존에 장착된 동일 부위 아이템이 있다면 해제
        if (equippedItems.ContainsKey(slot) && equippedItems[slot] != null)
        {
            Unequip(slot);
        }

        // 1. 장착 데이터 등록
        equippedItems[slot] = newItem;

        // 2. 무기 프리팹 씬 생성 (외형/소켓 연동)
        if (slot == EquipmentSlot.MainWeapon && newItem.weaponPrefab != null && mainHandSocket != null)
        {
            currentMainWeaponGO = Instantiate(newItem.weaponPrefab, mainHandSocket);
            currentMainWeaponGO.transform.localPosition = Vector3.zero;
            currentMainWeaponGO.transform.localRotation = Quaternion.identity;
        }

        // 3. 스탯 가산 (PlayerStatus 연동)
        if (playerStats != null)
        {
            playerStats.SetEquipmentBonus();
        }

        // 4. 이벤트 발행 (UI 업데이트 등)
        OnEquipmentChanged?.Invoke(slot, newItem);
    }

    /// <summary> 장비 해제 처리 </summary>
    public void Unequip(EquipmentSlot slot)
    {
        if (!equippedItems.ContainsKey(slot) || equippedItems[slot] == null) return;

        EquipmentItemData unequippedItem = equippedItems[slot];

        // 1. 씬 내 프리팹 파괴
        if (slot == EquipmentSlot.MainWeapon && currentMainWeaponGO != null)
        {
            Destroy(currentMainWeaponGO);
        }

        // 2. 스탯 감산
        if (playerStats != null)
        {
            playerStats.SetEquipmentBonus();
        }

        // 3. 데이터 제거
        equippedItems[slot] = null;

        // 4. 이벤트 발행
        OnEquipmentChanged?.Invoke(slot, null);
    }

    // --- 저장 / 불러오기용 메서드 ---

    /// <summary> SaveHandler가 저장할 때 호출: 부위별 장착된 아이템 ID 딕셔너리/리스트 반환 </summary>
    public Dictionary<string, string> GetEquippedItemIDs()
    {
        var saveDict = new Dictionary<string, string>();
        foreach (var pair in equippedItems)
        {
            if (pair.Value != null)
            {
                saveDict[pair.Key.ToString()] = pair.Value.itemId;
            }
        }
        return saveDict;
    }
    public WeaponItemData GetMainWeaponData()
    {

        return equippedItems.FirstOrDefault(x => x.Key == EquipmentSlot.MainWeapon).Value as WeaponItemData;
    }

    /// <summary> SaveHandler가 불러올 때 호출: 저장되었던 ID 기반으로 아이템 DB/Addressables에서 찾아 재장착 </summary>
    /// <summary> SaveHandler가 불러올 때 호출: 직렬화된 리스트 데이터를 바탕으로 장착 복원 </summary>
    public void LoadEquippedItems(List<EquippedItemSaveData> savedEquipList, EquipmentItemDatabaseSO itemDB)
    {
        if (savedEquipList == null || itemDB == null) return;

        foreach (var itemSaveData in savedEquipList)
        {
            if (Enum.TryParse<EquipmentSlot>(itemSaveData.slotName, out var slot))
            {
                EquipmentItemData equipmentItemData = itemDB.GetItemByID(itemSaveData.itemID);
                if (equipmentItemData != null&&( equipmentItemData.equipSlot!=EquipmentSlot.MainWeapon|| equipmentItemData.equipSlot != EquipmentSlot.SubWeapon))
                {
                    Equip(equipmentItemData);
                }
                else if(equipmentItemData != null && (equipmentItemData.equipSlot == EquipmentSlot.MainWeapon || equipmentItemData.equipSlot == EquipmentSlot.SubWeapon))
                {
                    Debug.Log("err");
                    Equip(equipmentItemData as WeaponItemData);
                }else if(equipmentItemData == null && (equipmentItemData.equipSlot == EquipmentSlot.MainWeapon || equipmentItemData.equipSlot == EquipmentSlot.SubWeapon))
                {
                    Debug.Log("ccc");
                    Equip(equipmentItemData as WeaponItemData);
                }
            }
        }
    }
    /// <summary> SaveHandler가 저장할 때 호출: 직렬화 가능한 List<EquippedItemSaveData> 생성 </summary>
    public List<EquippedItemSaveData> GetEquippedItemSaveData()
    {
        List<EquippedItemSaveData> saveDataList = new List<EquippedItemSaveData>();

        foreach (var pair in equippedItems)
        {
            if (pair.Value != null)
            {
                saveDataList.Add(new EquippedItemSaveData
                {
                    slotName = pair.Key.ToString(),
                    itemID = pair.Value.itemId // EquipmentItemData 내 정의된 itemID
                });
            }
        }

        return saveDataList;
    }
}