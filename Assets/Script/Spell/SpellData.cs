using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Video;
public enum ElementType { Physic, Water, Fire, Electric, Arcane }
public enum SpellType { Projtile, Particle }
public enum NodeGrade { Normal, Rare, Unique ,Legendary}

// 스킬(마법) 원본 데이터
[CreateAssetMenu(fileName = "SpellData", menuName = "Magic System/Spell Data")]
public class SpellData : ScriptableObject
{
    public string spellId;
    public string spellName;
    [TextArea(5,10)]
    public string spellDescription;
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

}
// 특수 노드 데이터
[CreateAssetMenu(fileName = "SpecialNodeData", menuName = "Magic System/Special Node")]
    public class SpecialNodeData : ScriptableObject
    {
        public int nodeId;
        public string nodeName;
        public string description;
        public NodeGrade grade;
        public float additionalMpCost; // 추가 마나 소모[cite: 1]
        public float additionalHpCost; // 희생 노드 등의 추가 체력 소모[cite: 1]

        // 노드별 특수 효과 연산 적용 메서드
        public void ApplyNodeEffect(SpellCastContext context) { /* ... */ }

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
    // 노드 추가 비용이 합산된 총 MP 소모량
    public float GetTotalMpCost()
        {
            float cost = baseData.baseMpCost;
            foreach (var node in equippedNodes)
            {
                if (node != null) cost += node.additionalMpCost;
            }
            return Mathf.Max(0, cost);
        }
    }