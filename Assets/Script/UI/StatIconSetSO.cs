using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatIconSet", menuName = "Arcane/Stat Icon Set")]
public class StatIconSetSO : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public StatIconType type;
        public Sprite icon;
    }

    [SerializeField] private List<Entry> entries = new List<Entry>();
    [SerializeField] private Sprite fallbackIcon;      // 풀에 없는 스탯일 때 표시

    private Dictionary<StatIconType, Sprite> cache;

    public Sprite GetIcon(StatIconType type)
    {
        if (cache == null) BuildCache();
        return cache.TryGetValue(type, out Sprite icon) && icon != null ? icon : fallbackIcon;
    }

    private void BuildCache()
    {
        cache = new Dictionary<StatIconType, Sprite>();
        foreach (var e in entries)
        {
            if (cache.ContainsKey(e.type))
            {
                Debug.LogWarning($"[StatIconSet] 중복된 타입: {e.type}", this);
                continue;
            }
            cache.Add(e.type, e.icon);
        }
    }

    // 에디터에서 수정할 때 캐시 갱신
    private void OnValidate() => cache = null;
}