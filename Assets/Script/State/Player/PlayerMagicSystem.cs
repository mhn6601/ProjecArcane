using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagicSystem : MonoBehaviour
{
    [Header("Quick Slots (전투 중 시전할 4개 스킬 슬롯)")]
    [SerializeField] private RuntimeSpellInstance[] quickSlots = new RuntimeSpellInstance[4];

    [Header("Unlocked Inventory (획득/해금한 스킬 및 노드 목록)")]
    private List<SpellData> unlockedSpells = new List<SpellData>();         // 해금된 스킬 목록
    private List<SpecialNodeData> unlockedNodes = new List<SpecialNodeData>(); // 해금된 특수 노드 목록

    public RuntimeSpellInstance[] QuickSlots => quickSlots;

    // UI 갱신용 이벤트
    public event Action OnSkillSetupChanged;

    /// <summary>
    /// 1. 마일스톤 달성/보스 처치 시 노드 또는 스킬 획득
    /// </summary>
    public void ObtainNode(SpecialNodeData newNode)
    {
        if (!unlockedNodes.Contains(newNode))
        {
            unlockedNodes.Add(newNode);
            OnSkillSetupChanged?.Invoke();
        }
    }

    /// <summary>
    /// 2. UI 조작: 특정 스킬 슬롯의 속성 변경 (비용 소모 없음)
    /// </summary>
    public void ChangeSlotElement(int slotIndex, ElementType newElement)
    {
        if (slotIndex < 0 || slotIndex >= quickSlots.Length) return;
        if (quickSlots[slotIndex] != null)
        {
            quickSlots[slotIndex].currentElement = newElement;
            OnSkillSetupChanged?.Invoke();
        }
    }

    /// <summary>
    /// 3. UI 조작: 스킬의 노드 소켓(0 or 1)에 특수 노드 장착/해제
    /// </summary>
    public void EquipNodeToSpell(int slotIndex, int nodeSocketIndex, SpecialNodeData nodeData)
    {
        if (slotIndex < 0 || slotIndex >= quickSlots.Length) return;
        if (nodeSocketIndex < 0 || nodeSocketIndex >= 2) return;

        if (quickSlots[slotIndex] != null)
        {
            quickSlots[slotIndex].equippedNodes[nodeSocketIndex] = nodeData;
            OnSkillSetupChanged?.Invoke();
        }
    }

    /// <summary>
    /// 4. 전투 입력: 퀵슬롯 스킬 시전 실행
    /// </summary>
    public void CastQuickSlotSpell(int slotIndex, Player player)
    {
        if (slotIndex < 0 || slotIndex >= quickSlots.Length) return;

        RuntimeSpellInstance spellInstance = quickSlots[slotIndex];
        if (spellInstance == null || spellInstance.baseData == null) return;

        float totalMpCost = spellInstance.GetTotalMpCost();
        
        // MP 차감 및 FSM 시전 상태 진입
        if (player.ResourceSystem.UseMana(totalMpCost))
        {
            // FSM Action State 변경 (시전 위치는 Player.MagicSocket 활용)
            player.Controller.ActionFSM .ChangeState(new PlayerSpellCastState(player,player.Animator, spellInstance)); 
        }
    }
}