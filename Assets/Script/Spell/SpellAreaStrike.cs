using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 폭발 하나의 설정. SpellData(기본 폭발), 스페셜 노드, 독자 범위폭발 마법이 공통으로 사용한다.
/// </summary>
[Serializable]
public class AreaStrikeConfig
{
    [Tooltip("AreaStrike 컴포넌트가 붙은 폭발 오브젝트 프리팹 (비어 있으면 폭발 없음)")]
    public GameObject prefab;
    public float radius = 3f;
    [Tooltip("폭발 데미지 = 기준 데미지 × 이 배율")]
    public float damageMultiplier = 1f;
}

/// <summary>
/// 폭발로 범위 데미지를 주는 오브젝트.
/// 생성 위치와 시점은 호출하는 쪽이 정한다 (투사체 적중 지점, 마법 시전 위치 등).
/// 프리팹 루트에 붙이고, 자식에 파티클 등 연출을 둔다.
/// </summary>
public class SpellAreaStrike : MonoBehaviour
{
    [Header("Targeting")]
    [Tooltip("피해를 줄 대상 레이어 (적 레이어)")]
    [SerializeField] private LayerMask targetLayers;

    [Header("Lifetime")]
    [Tooltip("폭발 후 오브젝트를 유지하는 시간 (연출이 끝날 때까지)")]
    [SerializeField] private float lifetime = 2f;

    [Header("Hit")]
    [SerializeField] private float staggerValue = 5f;

    [Header("Visual")]
    [Tooltip("반지름에 맞춰 스케일할 연출 루트 (비우면 이 오브젝트 자신)")]
    [SerializeField] private Transform visualRoot;
    [Tooltip("true면 지름 1 기준 연출을 반지름 × 2 로 스케일")]
    [SerializeField] private bool scaleVisualToRadius = true;
    [Tooltip("시전 속성에 맞는 연출을 자식으로 생성 (비우면 프리팹에 직접 넣은 연출만 사용)")]
    [SerializeField] private AreaStrikeVfxSet vfxSet;

    /// <summary>
    /// 설정대로 폭발 오브젝트를 생성하고 즉시 폭발시킨다.
    /// </summary>
    /// <param name="config">폭발 설정 (프리팹/반지름/배율)</param>
    /// <param name="position">폭발 중심 위치</param>
    /// <param name="context">시전 컨텍스트 (시전자, 속성)</param>
    /// <param name="baseDamage">기준 데미지 (최종 데미지 = 기준 × config.damageMultiplier)</param>
    /// <param name="excludeTarget">이미 직접 맞은 대상 (없으면 null)</param>
    public static SpellAreaStrike Spawn(AreaStrikeConfig config, Vector3 position, SpellCastContext context,
                                   float baseDamage, ICombatTarget excludeTarget = null)
    {
        if (config == null || config.prefab == null || config.radius <= 0f) return null;

        GameObject obj = Instantiate(config.prefab, position, Quaternion.identity);

        if (!obj.TryGetComponent<SpellAreaStrike>(out var strike))
        {
            Debug.LogWarning("[AreaStrike] 프리팹에 AreaStrike 컴포넌트가 없습니다.", obj);
            Destroy(obj);
            return null;
        }

        strike.Initialize(context, baseDamage * config.damageMultiplier, config.radius, excludeTarget);
        return strike;
    }

    public void Initialize(SpellCastContext context, float damage, float radius, ICombatTarget excludeTarget = null)
    {
        Transform root = visualRoot != null ? visualRoot : transform;

        if (scaleVisualToRadius)
        {
            root.localScale = Vector3.one * (radius * 2f);
        }

        // 시전 속성에 맞는 연출 생성 (루트의 자식이므로 반지름 스케일이 같이 적용됨)
        if (vfxSet != null)
        {
            GameObject vfx = vfxSet.GetVfx(context.Element);
            if (vfx != null) Instantiate(vfx, root, false);
        }

        Explode(context, damage, radius, excludeTarget);

        Destroy(gameObject, lifetime);
    }

    private void Explode(SpellCastContext context, float damage, float radius, ICombatTarget excludeTarget)
    {
        Transform attacker = context.Caster != null ? context.Caster.transform : transform;

        Collider[] cols = Physics.OverlapSphere(transform.position, radius, targetLayers);

        // 한 적에 콜라이더가 여러 개여도 1회만 피해
        var damaged = new HashSet<ICombatTarget>();
        if (excludeTarget != null) damaged.Add(excludeTarget);

        foreach (var col in cols)
        {
            if (context.Caster != null && col.transform.IsChildOf(context.Caster.transform)) continue;

            if (!col.TryGetComponent<ICombatTarget>(out var target))
                target = col.GetComponentInParent<ICombatTarget>();

            if (target == null || target.IsDead || !damaged.Add(target)) continue;

            AttackData attackData = new AttackData
            {
                damage = damage,
                element = context.Element,
                // 기타 AttackData 필드 바인딩
            };

            target.ResolveHit(attackData, attacker, transform, HitPart.Body);
            target.ApplyStagger(staggerValue);
        }
    }
}
