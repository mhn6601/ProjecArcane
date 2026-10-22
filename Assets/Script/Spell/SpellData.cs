using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Video;
public enum ElementType { Physic, Water, Fire, Electric, Arcane }
public enum SpellType { Projtile, Particle }
public enum NodeGrade { Normal, Rare, Unique, Legendary }

// 스킬(마법) 원본 데이터
[CreateAssetMenu(fileName = "SpellData", menuName = "Magic System/Spell Data")]
public class SpellData : ScriptableObject
{
    public string spellId;
    public string spellName;
    [TextArea(5, 10)]
    public string spellDescription;
    [Header("Tags")]
    public List<SpellTagSO> Tags = new List<SpellTagSO>();
    public VideoClip VideoClip;
    public AnimationClip animationClip;
    public Sprite Icon;
    public float baseDamage;
    public float spellPowerRatio;
    public float baseMpCost;
    public float cooldown;
    public ElementType elementType;

    // 속성별 비주얼/이펙트 프리팹 맵
    public GameObject VfxPrefab;
    [Header("AreaStrike")]
    public AreaStrikeConfig areaStrike;
}

// 플레이어가 세팅한 런타임 스킬 인스턴스
[System.Serializable]
public class RuntimeSpellInstance
{
    public SpellData baseData;
    public ElementType currentElement = ElementType.Water; // 기본 속성은 물
    public SpellType spellType;
    public SpecialNodeData[] equippedNodes = new SpecialNodeData[2]; // 최대 2개
    public RuntimeSpellInstance(SpellData data)
    {
        baseData = data;
        currentElement = data.elementType;   // 기본 속성은 스펠 에셋을 따름
    }
    public bool CanEquipNode(SpecialNodeData node)
    {
        return node != null && node.CanApplyTo(baseData);
    }

    public bool TryEquipNode(int slot, SpecialNodeData node)
    {
        if (slot < 0 || slot >= equippedNodes.Length) return false;
        if (!CanEquipNode(node)) return false;      // 태그 불일치면 거부
        equippedNodes[slot] = node;
        return true;
    }

    public void UnequipNode(int slot)
    {
        if (slot >= 0 && slot < equippedNodes.Length) equippedNodes[slot] = null;
    }

    // 스펠이 바뀌었거나 데이터가 수정됐을 때 호출 (불일치 노드 해제)
    public void RevalidateNodes()
    {
        for (int i = 0; i < equippedNodes.Length; i++)
            if (equippedNodes[i] != null && !equippedNodes[i].CanApplyTo(baseData))
                equippedNodes[i] = null;
    }

    // 발동 시점 안전장치: 태그가 맞는 노드만 효과 적용
    public void ApplyNodes(SpellCastContext ctx)
    {
        foreach (var node in equippedNodes)
            if (node != null && node.CanApplyTo(baseData))
                node.ApplyNodeEffect(ctx);
    }
    // 노드 추가 비용이 합산된 총 MP 소모량
    public float GetTotalMpCost()
    {
        float cost = baseData.baseMpCost;
        foreach (var node in equippedNodes)
        {
            if (node != null) cost += node.GetAditionalMpCost();
        }
        return Mathf.Max(0, cost);
    }
}