using UnityEngine;

[CreateAssetMenu(menuName = "Arcane/Spell Tag")]
public class SpellTagSO : ScriptableObject
{
    public string displayName;           // UI 안내용 (예: "투사체")
    [TextArea] public string description;
}