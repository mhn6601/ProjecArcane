using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Spell Database")]
public class SpellDatabaseSO : ScriptableObject
{
    [SerializeField] private List<SpellData> allSpells = new List<SpellData>();
    private Dictionary<string, SpellData> map;

    public SpellData Get(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        if (map == null)
        {
            map = new Dictionary<string, SpellData>();
            foreach (var spell in allSpells)
            {
                if (spell != null) map[spell.spellId] = spell;
            }
        }
        return map.TryGetValue(id, out var data) ? data : null;
    }
}