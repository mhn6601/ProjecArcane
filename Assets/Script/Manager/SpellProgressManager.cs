using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellProgressManager : Singleton<SpellProgressManager>
{
    [SerializeField] private SpellDatabaseSO database;

    private SpellSaveData saveData;

    public event Action<string> OnSpellUnlocked;          // spellId
    public event Action<int, SpellData> OnSlotChanged;    // 슬롯 인덱스(0~3), 장착된 스펠(비면 null)
    public event Action OnSpecialChanged;

    protected override void Awake()
    {
        base.Awake();
        saveData = SpellSaveSystem.Load();
        Sanitize();
    }

    // 세이브가 오염됐을 때(길이 불일치, 삭제된 스펠 참조 등) 정리
    private void Sanitize()
    {
       saveData.acquiredSpecialIds ??= new List<string>();
       saveData.specialSlots ??= new List<SpecialSlotEntry>();
        if (saveData.equippedSpellIds == null || saveData.equippedSpellIds.Length != SpellSaveData.SlotCount)
        {
            var fixedSlots = new string[SpellSaveData.SlotCount];
            for (int i = 0; i < fixedSlots.Length; i++)
            {
                fixedSlots[i] = (saveData.equippedSpellIds != null && i < saveData.equippedSpellIds.Length)
                    ? saveData.equippedSpellIds[i] ?? "" : "";
            }
            saveData.equippedSpellIds = fixedSlots;
        }

        for (int i = 0; i < saveData.equippedSpellIds.Length; i++)
        {
            string id = saveData.equippedSpellIds[i];
            // 해금 안 됐거나 DB에 없는 스펠이 장착돼 있으면 비움
            if (!string.IsNullOrEmpty(id) && (!IsUnlocked(id, SpellNodeType.Spell) || database.Get(id) == null))
                saveData.equippedSpellIds[i] = "";
        }
        saveData.unlockedStatNodeIds = saveData.unlockedStatNodeIds.Distinct().ToList();
    }

    // -------------------------
    // 해금
    // -------------------------
    public bool IsUnlocked(string spellId, SpellNodeType type)
    {
        if (string.IsNullOrEmpty(spellId)) return false;
        switch (type)
        {
            case SpellNodeType.Spell:
                return saveData.unlockedSpellIds.Contains(spellId);
            case SpellNodeType.Stat:
                return saveData.unlockedStatNodeIds.Contains(spellId);
            default:
                return false;
        }
    }

    public bool Unlock(string spellId,SpellNodeType type)
    {
        switch (type)
        {
            case SpellNodeType.Spell:
                if (string.IsNullOrEmpty(spellId) || IsUnlocked(spellId, SpellNodeType.Spell)) return false;

                saveData.unlockedSpellIds.Add(spellId);
                OnSpellUnlocked?.Invoke(spellId);
                Save();
                return true;
            case SpellNodeType.Stat:
                if (string.IsNullOrEmpty(spellId) || IsUnlocked(spellId, SpellNodeType.Stat)) return false;

                saveData.unlockedStatNodeIds.Add(spellId);
                OnSpellUnlocked?.Invoke(spellId);
                Save();
                return true;
            case SpellNodeType.Special:
                return false;

            default:
                break;
        }
        return true;
    }

    // -------------------------
    // 장착 (slotIndex: 0~3 = 키 1~4)
    // -------------------------
    public int FindSlot(string spellId)
    {
        if (string.IsNullOrEmpty(spellId)) return -1;

        for (int i = 0; i < SpellSaveData.SlotCount; i++)
        {
            if (saveData.equippedSpellIds[i] == spellId) return i;
        }
        return -1;
    }

    // 스펠 트리에서 스펠을 슬롯에 장착. 이미 다른 슬롯에 있으면 서로 교환
    public SpellData GetEquipped(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return null;
        return database.Get(saveData.equippedSpellIds[slotIndex]);
    }
    public bool Equip(int slotIndex, SpellData spell)
    {
        if (!IsValidSlot(slotIndex) || spell == null) return false;
        if (!IsUnlocked(spell.spellId, SpellNodeType.Spell)) return false;

        int existing = FindSlot(spell.spellId);
        if (existing == slotIndex) return true;

        string previous = saveData.equippedSpellIds[slotIndex];   // 대상 슬롯에 있던 스펠
        saveData.equippedSpellIds[slotIndex] = spell.spellId;

        if (existing >= 0)
        {
            // 다른 슬롯에서 옮겨 온 경우: 그 슬롯에는 대상 슬롯에 있던 스펠이 들어감 (비어 있었다면 빈 슬롯)
            saveData.equippedSpellIds[existing] = previous;
            OnSlotChanged?.Invoke(existing, database.Get(previous));
        }

        OnSlotChanged?.Invoke(slotIndex, spell);
        Save();
        return true;
    }
    public void Unequip(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return;
        if (string.IsNullOrEmpty(saveData.equippedSpellIds[slotIndex])) return;

        saveData.equippedSpellIds[slotIndex] = "";
        OnSlotChanged?.Invoke(slotIndex, null);
        Save();
    }
    // 슬롯 두 칸의 스펠을 맞교환 (빈 슬롯과 교환하면 이동)
    public void SwapSlots(int a, int b)
    {
        if (!IsValidSlot(a) || !IsValidSlot(b) || a == b) return;

        string temp = saveData.equippedSpellIds[a];
        saveData.equippedSpellIds[a] = saveData.equippedSpellIds[b];
        saveData.equippedSpellIds[b] = temp;

        OnSlotChanged?.Invoke(a, GetEquipped(a));
        OnSlotChanged?.Invoke(b, GetEquipped(b));
        Save();
    }

    private static bool IsValidSlot(int i) => i >= 0 && i < SpellSaveData.SlotCount;
    // -------------------------
    // 스페셜 노드 (획득 / 장착)
    // -------------------------
    public bool IsSpecialAcquired(string specialId) =>
        saveData.acquiredSpecialIds.Contains(specialId);

    public bool AcquireSpecial(string specialId)
    {
        if (string.IsNullOrEmpty(specialId) || IsSpecialAcquired(specialId)) return false;

        saveData.acquiredSpecialIds.Add(specialId);
        OnSpecialChanged?.Invoke();
        Save();
        return true;
    }

    public string GetSpecialSlot(string spellId, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SpecialSlotEntry.SlotCount) return "";

        var entry = saveData.specialSlots.Find(e => e.spellId == spellId);
        return entry != null ? entry.specialIds[slotIndex] : "";
    }

    public bool EquipSpecial(string spellId, int slotIndex, string specialId)
    {
        if (slotIndex < 0 || slotIndex >= SpecialSlotEntry.SlotCount) return false;
        if (!IsUnlocked(spellId, SpellNodeType.Special) || !IsSpecialAcquired(specialId)) return false;

        // 같은 스페셜이 다른 슬롯에 있으면 비움 (아이템 1개 = 1곳에만 장착)
        foreach (var e in saveData.specialSlots)
        {
            for (int i = 0; i < e.specialIds.Length; i++)
            {
                if (e.specialIds[i] == specialId) e.specialIds[i] = "";
            }
        }

        var entry = saveData.specialSlots.Find(e => e.spellId == spellId);
        if (entry == null)
        {
            entry = new SpecialSlotEntry { spellId = spellId };
            saveData.specialSlots.Add(entry);
        }

        entry.specialIds[slotIndex] = specialId;
        OnSpecialChanged?.Invoke();
        Save();
        return true;
    }

    public void UnequipSpecial(string spellId, int slotIndex)
    {
        var entry = saveData.specialSlots.Find(e => e.spellId == spellId);
        if (entry == null || string.IsNullOrEmpty(entry.specialIds[slotIndex])) return;

        entry.specialIds[slotIndex] = "";
        OnSpecialChanged?.Invoke();
        Save();
    }

    // -------------------------
    // 저장
    // -------------------------
    public void Save() => SpellSaveSystem.Save(saveData);

    private void OnApplicationPause(bool paused) { if (paused) Save(); }
    private void OnApplicationQuit() => Save();
}