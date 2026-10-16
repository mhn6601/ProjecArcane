using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpellProgressManager : Singleton<SpellProgressManager>
{
    [SerializeField] private SpellDatabaseSO database;

    private SpellSaveData spellSavesaveData;
    // [수정] 사용하지 않던 specialNodeSaveData 필드 제거

    public event Action<string> OnSpellUnlocked;          // spellId
    public event Action<int, SpellData> OnSlotChanged;    // 슬롯 인덱스(0~3), 장착된 스펠(비면 null)
    public event Action OnSpecialChanged; 

    protected override void Awake()
    {
        base.Awake();
        spellSavesaveData = SpellSaveSystem.Load();
        Sanitize();
    }

    // 세이브가 오염됐을 때(길이 불일치, 삭제된 스펠 참조 등) 정리
    private void Sanitize()
    {
        spellSavesaveData.acquiredSpecialIds ??= new List<string>();
        spellSavesaveData.specialSlots ??= new List<SpecialSlotEntry>();
        if (spellSavesaveData.equippedSpellIds == null || spellSavesaveData.equippedSpellIds.Length != SpellSaveData.SlotCount)
        {
            var fixedSlots = new string[SpellSaveData.SlotCount];
            for (int i = 0; i < fixedSlots.Length; i++)
            {
                fixedSlots[i] = (spellSavesaveData.equippedSpellIds != null && i < spellSavesaveData.equippedSpellIds.Length)
                    ? spellSavesaveData.equippedSpellIds[i] ?? "" : "";
            }
            spellSavesaveData.equippedSpellIds = fixedSlots;
        }

        for (int i = 0; i < spellSavesaveData.equippedSpellIds.Length; i++)
        {
            string id = spellSavesaveData.equippedSpellIds[i];
            // 해금 안 됐거나 DB에 없는 스펠이 장착돼 있으면 비움
            if (!string.IsNullOrEmpty(id) && (!IsUnlocked(id, SpellNodeType.Spell) || database.Get(id) == null))
                spellSavesaveData.equippedSpellIds[i] = "";
        }

        foreach (var entry in spellSavesaveData.specialSlots)
        {
            if (entry == null) continue;

            // [수정] 스페셜 슬롯 배열이 null이거나 길이가 다르면 보정
            EnsureEntryArray(entry);

            SpellData spell = database.Get(entry.spellId);
            if (spell == null) continue;

            for (int i = 0; i < entry.specialIds.Length; i++)
            {
                // [수정] 두 DB 모두에서 조회. 못 찾으면(null) 건드리지 않음
                SpecialNodeData node = ResolveSpecial(entry.specialIds[i]);
                if (node != null && !node.CanApplyTo(spell))
                    entry.specialIds[i] = "";
            }
        }
        spellSavesaveData.unlockedStatNodeIds = spellSavesaveData.unlockedStatNodeIds.Distinct().ToList();
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
                return spellSavesaveData.unlockedSpellIds.Contains(spellId);
            case SpellNodeType.Stat:
                return spellSavesaveData.unlockedStatNodeIds.Contains(spellId);
            default:
                return false;
        }
    }

    public bool Unlock(string spellId, SpellNodeType type)
    {
        switch (type)
        {
            case SpellNodeType.Spell:
                if (string.IsNullOrEmpty(spellId) || IsUnlocked(spellId, SpellNodeType.Spell)) return false;

                spellSavesaveData.unlockedSpellIds.Add(spellId);
                OnSpellUnlocked?.Invoke(spellId);
                Save();
                return true;
            case SpellNodeType.Stat:
                if (string.IsNullOrEmpty(spellId) || IsUnlocked(spellId, SpellNodeType.Stat)) return false;

                spellSavesaveData.unlockedStatNodeIds.Add(spellId);
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
            if (spellSavesaveData.equippedSpellIds[i] == spellId) return i;
        }
        return -1;
    }

    // 스펠 트리에서 스펠을 슬롯에 장착. 이미 다른 슬롯에 있으면 서로 교환
    public SpellData GetEquipped(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return null;
        return database.Get(spellSavesaveData.equippedSpellIds[slotIndex]);
    }

    public bool Equip(int slotIndex, SpellData spell)
    {
        if (!IsValidSlot(slotIndex) || spell == null) return false;
        if (!IsUnlocked(spell.spellId, SpellNodeType.Spell)) return false;

        int existing = FindSlot(spell.spellId);
        if (existing == slotIndex) return true;

        string previous = spellSavesaveData.equippedSpellIds[slotIndex];   // 대상 슬롯에 있던 스펠
        spellSavesaveData.equippedSpellIds[slotIndex] = spell.spellId;

        if (existing >= 0)
        {
            // 다른 슬롯에서 옮겨 온 경우: 그 슬롯에는 대상 슬롯에 있던 스펠이 들어감 (비어 있었다면 빈 슬롯)
            spellSavesaveData.equippedSpellIds[existing] = previous;
            OnSlotChanged?.Invoke(existing, database.Get(previous));
        }

        OnSlotChanged?.Invoke(slotIndex, spell);
        Save();
        return true;
    }

    public void Unequip(int slotIndex)
    {
        if (!IsValidSlot(slotIndex)) return;
        if (string.IsNullOrEmpty(spellSavesaveData.equippedSpellIds[slotIndex])) return;

        spellSavesaveData.equippedSpellIds[slotIndex] = "";
        OnSlotChanged?.Invoke(slotIndex, null);
        Save();
    }

    // 슬롯 두 칸의 스펠을 맞교환 (빈 슬롯과 교환하면 이동)
    public void SwapSlots(int a, int b)
    {
        if (!IsValidSlot(a) || !IsValidSlot(b) || a == b) return;

        string temp = spellSavesaveData.equippedSpellIds[a];
        spellSavesaveData.equippedSpellIds[a] = spellSavesaveData.equippedSpellIds[b];
        spellSavesaveData.equippedSpellIds[b] = temp;

        OnSlotChanged?.Invoke(a, GetEquipped(a));
        OnSlotChanged?.Invoke(b, GetEquipped(b));
        Save();
    }

    private static bool IsValidSlot(int i) => i >= 0 && i < SpellSaveData.SlotCount;

    // -------------------------
    // 스페셜 노드 (보유 / 장착)
    // -------------------------
    public SpellData GetSpellData(string spellId) => database.Get(spellId);

    public SpecialNodeData GetSpecialData(string specialId) => ResolveSpecial(specialId);

    // [수정] 스페셜 노드 조회: 스페셜 인벤토리 DB를 먼저, 없으면 스펠 DB에서 찾음
    private SpecialNodeData ResolveSpecial(string specialId)
    {
        if (string.IsNullOrEmpty(specialId)) return null;

        var inv = SpecialNodeInventoryManager.Instance;
        SpecialNodeData data = inv != null ? inv.GetNodeData(specialId) : null;
        return data != null ? data : database.GetSpecial(specialId);
    }

    // [수정] 보유 목록을 acquiredSpecialIds가 아니라 스페셜 노드 인벤토리에서 가져옴
    public IEnumerable<SpecialNodeData> GetAcquiredSpecials()
    {
        var inv = SpecialNodeInventoryManager.Instance;
        if (inv != null)
        {
            foreach (var instance in inv.SpecialNodeInventory)
                if (instance != null && instance.Data != null) yield return instance.Data;
            yield break;
        }

        foreach (var id in spellSavesaveData.acquiredSpecialIds)
        {
            var node = database.GetSpecial(id);
            if (node != null) yield return node;
        }
    }

    // [수정] 보유 판정도 스페셜 노드 인벤토리 기준 (기존: acquiredSpecialIds에만 의존해서 항상 false였음)
    public bool IsSpecialAcquired(string specialId)
    {
        if (string.IsNullOrEmpty(specialId)) return false;

        var inv = SpecialNodeInventoryManager.Instance;
        if (inv != null) return inv.HasNodeID(specialId);

        return spellSavesaveData.acquiredSpecialIds.Contains(specialId);
    }

    // 특정 스페셜이 장착된 위치 (없으면 false)
    public bool FindSpecialLocation(string specialId, out string spellId, out int slotIndex)
    {
        spellId = null; slotIndex = -1;
        if (string.IsNullOrEmpty(specialId)) return false;

        foreach (var e in spellSavesaveData.specialSlots)
        {
            if (e == null || e.specialIds == null) continue;   // [수정] null 방어

            for (int i = 0; i < e.specialIds.Length; i++)
                if (e.specialIds[i] == specialId) { spellId = e.spellId; slotIndex = i; return true; }
        }
        return false;
    }

    // [수정] 엔트리의 배열이 null이거나 길이가 맞지 않으면 보정
    private static void EnsureEntryArray(SpecialSlotEntry entry)
    {
        if (entry.specialIds != null && entry.specialIds.Length == SpecialSlotEntry.SlotCount) return;

        var fixedIds = new string[SpecialSlotEntry.SlotCount];
        for (int i = 0; i < fixedIds.Length; i++)
        {
            fixedIds[i] = (entry.specialIds != null && i < entry.specialIds.Length)
                ? entry.specialIds[i] ?? "" : "";
        }
        entry.specialIds = fixedIds;
    }

    private void SetSpecialRaw(string spellId, int slotIndex, string specialId)
    {
        var entry = spellSavesaveData.specialSlots.Find(e => e != null && e.spellId == spellId);
        if (entry == null)
        {
            entry = new SpecialSlotEntry { spellId = spellId };
            spellSavesaveData.specialSlots.Add(entry);
        }
        EnsureEntryArray(entry);   // [수정] 새 엔트리의 배열이 null이어도 안전
        entry.specialIds[slotIndex] = specialId;
    }

    /// <summary>장착. 선택한 노드가 다른 자리에 장착 중이면 두 자리를 교환</summary>
    public bool EquipOrSwapSpecial(string spellId, int slotIndex, string specialId)
    {
        if (slotIndex < 0 || slotIndex >= SpecialSlotEntry.SlotCount) return false;
        if (!IsUnlocked(spellId, SpellNodeType.Spell) || !IsSpecialAcquired(specialId)) return false;
        if (!CanEquipSpecial(spellId, specialId)) return false;

        string previous = GetSpecialSlot(spellId, slotIndex);   // 대상 자리에 있던 노드
        if (previous == specialId) return true;

        bool fromElsewhere = FindSpecialLocation(specialId, out string otherSpell, out int otherSlot);

        // 교환 시 상대 자리(다른 스펠)에 태그가 안 맞는 노드가 들어가게 되면 거부
        if (fromElsewhere && !string.IsNullOrEmpty(previous) && !CanEquipSpecial(otherSpell, previous))
            return false;

        if (fromElsewhere) SetSpecialRaw(otherSpell, otherSlot, previous);   // 상대 자리에 이전 노드(없으면 빈칸)
        SetSpecialRaw(spellId, slotIndex, specialId);

        OnSpecialChanged?.Invoke();
        Save();
        return true;
    }

    // [수정] 새로 추가: 스페셜 노드 장착 해제
    public bool UnequipSpecial(string spellId, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SpecialSlotEntry.SlotCount) return false;
        if (string.IsNullOrEmpty(GetSpecialSlot(spellId, slotIndex))) return false;

        SetSpecialRaw(spellId, slotIndex, "");
        OnSpecialChanged?.Invoke();
        Save();
        return true;
    }

    // [수정] 새로 추가: 해당 자리에 장착된 스페셜 노드 데이터 (없으면 null)
    public SpecialNodeData GetSpecialSlotData(string spellId, int slotIndex)
    {
        return ResolveSpecial(GetSpecialSlot(spellId, slotIndex));
    }

    // [수정] 새로 추가: 보유하지 않은 스페셜 노드가 장착돼 있으면 비움
    // SpecialNodeInventoryManager.LoadSpecialNodeInventory 끝에서 호출
    public void ValidateSpecialEquips()
    {
        bool changed = false;

        foreach (var entry in spellSavesaveData.specialSlots)
        {
            if (entry == null) continue;
            EnsureEntryArray(entry);

            for (int i = 0; i < entry.specialIds.Length; i++)
            {
                string id = entry.specialIds[i];
                if (!string.IsNullOrEmpty(id) && !IsSpecialAcquired(id))
                {
                    entry.specialIds[i] = "";
                    changed = true;
                }
            }
        }

        if (changed)
        {
            OnSpecialChanged?.Invoke();
            Save();
        }
    }

    public string GetSpecialSlot(string spellId, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SpecialSlotEntry.SlotCount) return "";

        var entry = spellSavesaveData.specialSlots.Find(e => e != null && e.spellId == spellId);
        if (entry == null || entry.specialIds == null || slotIndex >= entry.specialIds.Length)   // [수정] null/길이 방어
            return "";

        return entry.specialIds[slotIndex] ?? "";
    }

    public bool CanEquipSpecial(string spellId, string specialId)
    {
        SpellData spell = database.Get(spellId);
        SpecialNodeData node = ResolveSpecial(specialId);   // [수정] 두 DB 모두 조회
        return spell != null && node != null && node.CanApplyTo(spell);
    }

    // 슬롯의 스펠 + 장착된 스페셜 노드로 런타임 인스턴스 생성
    public RuntimeSpellInstance GetRuntimeInstance(int slotIndex)
    {
        SpellData spell = GetEquipped(slotIndex);
        if (spell == null) return null;

        var instance = new RuntimeSpellInstance(spell);
        for (int i = 0; i < instance.equippedNodes.Length; i++)
        {
            string id = GetSpecialSlot(spell.spellId, i);
            if (string.IsNullOrEmpty(id)) continue;

            instance.TryEquipNode(i, ResolveSpecial(id));   // 태그 불일치/없는 노드는 자동 제외
        }
        return instance;
    }

    // -------------------------
    // 저장
    // -------------------------
    public void Save()
    {
        SpellSaveSystem.Save(spellSavesaveData);
    }

    private void OnApplicationPause(bool paused) { if (paused) Save(); }
    private void OnApplicationQuit() => Save();
}
