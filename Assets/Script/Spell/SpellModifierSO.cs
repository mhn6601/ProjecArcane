using System.Collections.Generic;
using System.Linq;
using Unity.AppUI.UI;
using UnityEngine;

public enum TagMatchMode { All, Any }
public struct SpellShot
{
    public Vector3 position;
    public Quaternion rotation;
    public float damageMultiplier;
}



[CreateAssetMenu(fileName = "SpecialNodeData", menuName = "Magic System/Special Node")]
public class SpecialNodeData : ScriptableObject
{
    public string instanceID;
    public string nodeId;
    public string nodeName;
    public string description;
    public Sprite icon;
    public NodeGrade grade;

    [Header("Compatibility")]
    [Tooltip("스펠이 가져야 하는 태그 (비어 있으면 조건 없음)")]
    public List<SpellTagSO> requiredTags = new List<SpellTagSO>();
    [Tooltip("All: 모두 가져야 함 / Any: 하나라도 있으면 됨")]
    public TagMatchMode matchMode = TagMatchMode.All;
    [Tooltip("스펠이 이 태그 중 하나라도 가지면 장착/발동 불가")]
    public List<SpellTagSO> excludedTags = new List<SpellTagSO>();


    public bool CanApplyTo(SpellData spell)
    {
        if (spell == null) return false;

        // 1. 제외 태그: 하나라도 있으면 즉시 불가
        foreach (var tag in excludedTags)
            if (tag != null && spell.Tags.Contains(tag)) return false;

        // 2. 필요 태그: 비어 있으면 통과
        if (requiredTags.Count == 0) return true;

        foreach (var tag in requiredTags)
        {
            bool has = spell.Tags.Contains(tag);
            if (matchMode == TagMatchMode.Any && has) return true;
            if (matchMode == TagMatchMode.All && !has) return false;
        }
        return matchMode == TagMatchMode.All;
    }

    public string GetRequirementText()
    {
        var parts = new List<string>();

        if (requiredTags.Count > 0)
        {
            string sep = matchMode == TagMatchMode.All ? " + " : " / ";
            parts.Add("필요: " + string.Join(sep, requiredTags.Select(t => t.displayName)));
        }
        if (excludedTags.Count > 0)
            parts.Add("불가: " + string.Join(", ", excludedTags.Select(t => t.displayName)));

        return parts.Count > 0 ? string.Join("  |  ", parts) : "제한 없음";
    }

    // 하위 클래스에서 override. 기본 노드는 효과 없음
    public virtual void ApplyNodeEffect(SpellCastContext ctx) { }
    public virtual float GetAditionalMpCost() { return 0f; }
}