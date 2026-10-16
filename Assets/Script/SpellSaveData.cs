using System;
using System.Collections.Generic;

[Serializable]
public class SpecialSlotEntry
{
    public const int SlotCount = 2;

    public string spellId;
    public string[] specialIds = new string[SlotCount] { "", "" };
}

[Serializable]
public class SpellSaveData
{
    public const int SlotCount = 4;

    public List<string> unlockedSpellIds = new List<string>();
    public string[] equippedSpellIds = new string[SlotCount] { "", "", "", "" };
    public List<string> unlockedStatNodeIds = new List<string>();

    // 추가: 게임 진행 중 획득한 스페셜 아이템 ID
    public List<string> acquiredSpecialIds = new List<string>();

    // 추가: 스킬별 특수 슬롯 2개의 장착 상태
    public List<SpecialSlotEntry> specialSlots = new List<SpecialSlotEntry>();
}
