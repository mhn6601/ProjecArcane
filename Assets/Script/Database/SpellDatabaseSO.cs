using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Spell Database")]
public class SpellDatabaseSO : ScriptableObject
{
    [SerializeField] private List<SpellData> allSpells = new List<SpellData>();
    [SerializeField] private List<SpecialNodeData> allSpecials = new List<SpecialNodeData>();

    private Dictionary<string, SpellData> map;
    private Dictionary<string, SpecialNodeData> specialMap;

    // 에디터에서 리스트를 수정하거나 플레이를 다시 시작해도 오래된 캐시가 남지 않도록 초기화
    private void OnEnable()
    {
        map = null;
        specialMap = null;
    }

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

    // 세이브에 저장되는 스페셜 id 문자열 규칙. 다른 규칙을 쓰고 있다면 이 한 줄만 수정
    public static string GetSpecialKey(SpecialNodeData node) => node.nodeId.ToString();

    public SpecialNodeData GetSpecial(string specialId)
    {
        if (string.IsNullOrEmpty(specialId)) return null;

        if (specialMap == null)
        {
            specialMap = new Dictionary<string, SpecialNodeData>();
            foreach (var special in allSpecials)
            {
                if (special == null) continue;

                string key = GetSpecialKey(special);
                if (specialMap.ContainsKey(key))
                    Debug.LogWarning($"[SpellDatabaseSO] 중복된 스페셜 id: {key} ({special.name})");
                else
                    specialMap[key] = special;
            }
        }
        return specialMap.TryGetValue(specialId, out var data) ? data : null;
    }
}