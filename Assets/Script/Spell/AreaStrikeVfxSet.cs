using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 속성(ElementType)별 폭발 연출 프리팹 모음.
/// AreaStrike 프리팹이 하나만 있어도 시전 속성에 맞는 이펙트가 자동으로 붙도록 한다.
/// </summary>
[CreateAssetMenu(fileName = "AreaStrikeVfxSet", menuName = "Magic System/AreaStrike Vfx Set")]
public class AreaStrikeVfxSet : ScriptableObject
{
    [Serializable]
    public struct Entry
    {
        public ElementType element;
        public GameObject vfxPrefab;
    }

    [Tooltip("해당 속성 항목이 없을 때 사용할 기본 연출 (비워도 됨)")]
    [SerializeField] private GameObject defaultVfx;
    [SerializeField] private List<Entry> entries = new List<Entry>();

    public GameObject GetVfx(ElementType element)
    {
        foreach (var e in entries)
        {
            if (e.element == element && e.vfxPrefab != null) return e.vfxPrefab;
        }
        return defaultVfx;
    }
}
