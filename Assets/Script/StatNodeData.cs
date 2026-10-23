using UnityEngine;
public enum StatIconType
{
    Intelligence,   // 지능
    Strength,       // 힘
    Vitality        // 체력
}
[CreateAssetMenu(menuName = "Spell/Stat Node Data")]
public class StatNodeData : ScriptableObject
{
    public string nodeId;                 // 고유 ID. 스펠 ID와 겹치지 않게 "stat_001" 형식 권장
    public string nodeName;
    [TextArea(2, 5)] public string description;
    public StatIconType iconType;
    public StatBonus[] bonuses;
}