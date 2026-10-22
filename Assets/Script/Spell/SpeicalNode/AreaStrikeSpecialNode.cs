using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AreaStrikeSpecialNode", menuName = "Special Node/Area Strike")]
public class AreaStrikeSpecialNode : SpecialNodeData
{
    [Header("Area Strike")]
    [Tooltip("폭발 반지름")]
    public float radius = 3f;
    [Tooltip("폭발 데미지 = 기준 데미지 × 이 배율")]
    [Range(0f, 2f)] public float damageMultiplier = 0.5f;
    [Tooltip("AreaStrike 컴포넌트가 붙은 폭발 오브젝트 프리팹")]
    [FormerlySerializedAs("vfxPrefab")]
    public GameObject areaStrikePrefab;

    public override void ApplyNodeEffect(SpellCastContext ctx)
    {
        Debug.Log($"[AreaStrike] {name} / prefab={(areaStrikePrefab != null ? areaStrikePrefab.name : "null")}", this);
        if (areaStrikePrefab == null) return;

        // 스펠 기본 폭발이 있어도 덮어쓰지 않고 폭발을 하나 더 추가한다
        ctx.AreaStrikes.Add(new AreaStrikeConfig
        {
            prefab = areaStrikePrefab,
            radius = radius,
            damageMultiplier = damageMultiplier
        });
    }
}
