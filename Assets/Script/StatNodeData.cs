using UnityEngine;

[CreateAssetMenu(menuName = "Spell/Stat Node Data")]
public class StatNodeData : ScriptableObject
{
    public string nodeId;                 // 고유 ID. 스펠 ID와 겹치지 않게 "stat_001" 형식 권장
    public string nodeName;
    [TextArea(2, 5)] public string description;
    public Sprite icon;
    public StatBonus[] bonuses;
}