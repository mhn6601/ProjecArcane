using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class SpellProjectile : MonoBehaviour
{
    [Header("Impact VFX")]
    [SerializeField] private GameObject defaultHitVfx;
    [SerializeField] private GameObject shadowVfxPrefab; // 잔영 노드용 장판 이펙트

    [Header("Targeting")]
    [FormerlySerializedAs("ricochetTargetLayers")]
    [SerializeField] private LayerMask targetLayers; // 연쇄 대상 탐색용 (적 레이어). 폭발의 대상 레이어는 AreaStrike 프리팹에서 설정

    private SpellCastContext context;
    private Vector3 moveDirection;
    private float currentLifetime = 0f;
    private float maxLifetime = 5f; // 사거리 및 연장 노드 영향
    private bool isInitialized = false;

    // 연쇄 런타임 상태: ctx는 분열 투사체끼리 공유될 수 있으므로 투사체 로컬로 복사해서 사용
    private int remainingBounces;
    private float currentDamage;
    private Transform homingTarget;

    // 콜라이더 단위 중복 피격 방지
    private readonly HashSet<Collider> hitColliders = new HashSet<Collider>();
    // 대상 단위 중복 피격 방지 (한 적의 자식 콜라이더 여러 개 / 연쇄 왕복 방지)
    private readonly HashSet<ICombatTarget> hitTargets = new HashSet<ICombatTarget>();

    /// <summary>
    /// FSM 시전 상태에서 Instantiate 직후 호출하여 Context 정보 주입
    /// </summary>
    public void Initialize(SpellCastContext context)
    {
        this.context = context;
        this.moveDirection = context.CastDirection;
        this.isInitialized = true;

        // 연쇄 상태 초기화
        remainingBounces = context.RecocehtCount;
        currentDamage = context.FinalDamage;
        homingTarget = context.TargetTransform;

        // 1. 크기 배율 적용 (확장 노드)
        transform.localScale *= context.ScaleMultiplier;

        // 2. 사거리/수명 연장 적용 (연장 노드)
        maxLifetime *= (context.ScaleMultiplier > 1f) ? 1.5f : 1f;

        // 3. 트리거 설정
        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        // 1. 이동 처리 (추적/유도: 연쇄 후에는 새 타깃을 추적)
        if (context.HasHoming && homingTarget != null)
        {
            Vector3 targetDir = (homingTarget.position - transform.position).normalized;
            moveDirection = Vector3.Slerp(moveDirection, targetDir, Time.deltaTime * 5f);
        }

        transform.position += moveDirection * context.ProjectileSpeed * Time.deltaTime;
        transform.forward = moveDirection;

        // 2. 수명/사거리 체크
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= maxLifetime)
        {
            OnExpire(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isInitialized) return;

        // 시전자 및 중복 피격 대상 방지
        if (other.gameObject == context.Caster || hitColliders.Contains(other)) return;

        if (!other.TryGetComponent<ICombatTarget>(out var target))
        {
            target = other.GetComponentInParent<ICombatTarget>();
        }

        if (target == null || target.IsDead) return;

        hitColliders.Add(other);

        // 이미 맞은 대상의 다른 콜라이더면 무시
        if (!hitTargets.Add(target)) return;

        AttackData attackData = new AttackData
        {
            damage = currentDamage,
            element = context.Element,
            // 기타 AttackData 필드 바인딩
        };

        target.ResolveHit(attackData, context.Caster.transform, transform, HitPart.Body);
        target.ApplyStagger(10f); // 예시 경직 수치

        SpawnHitVfx();

        // 추가 범위타격 (연쇄 판정보다 먼저: 현재 적중 데미지 기준)
        TriggerAreaStrike(target);

        // 연쇄 가능하면 소멸하지 않고 다음 대상으로 재비행
        if (TryBounce(other)) return;

        OnExpire(true);
    }

    /// <summary>
    /// 적중 지점 기준 범위 내에서 아직 맞지 않은 가장 가까운 적을 찾아 방향을 전환한다.
    /// </summary>
    private bool TryBounce(Collider hitCollider)
    {
        if (remainingBounces <= 0 || context.RecocehtRange <= 0f) return false;

        Vector3 origin = hitCollider.bounds.center;
        Collider[] cols = Physics.OverlapSphere(origin, context.RecocehtRange, targetLayers);

        Collider best = null;
        float bestSqr = float.MaxValue;

        foreach (var col in cols)
        {
            if (col == hitCollider) continue;
            if (col.transform.IsChildOf(context.Caster.transform)) continue;

            if (!col.TryGetComponent<ICombatTarget>(out var t))
                t = col.GetComponentInParent<ICombatTarget>();

            if (t == null || t.IsDead || hitTargets.Contains(t)) continue;

            float sqr = (col.bounds.center - origin).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = col;
            }
        }

        if (best == null) return false;

        remainingBounces--;
        currentDamage *= context.RecocehtDamageMultiplier;
        homingTarget = best.transform;

        moveDirection = (best.bounds.center - transform.position).normalized;
        transform.forward = moveDirection;
        currentLifetime = 0f; // 새 비행 구간이므로 수명 리셋

        return true;
    }

    /// <summary>
    /// 이 시전에 폭발이 붙어 있으면(스펠 기본 / 스페셜 노드) 적중 지점마다 생성한다.
    /// 폭발이 없으면 아무 일도 하지 않는다. 직접 맞은 대상은 제외한다.
    /// 폭발에 맞은 적은 hitTargets에 넣지 않으므로 연쇄 후보에는 그대로 남는다.
    /// </summary>
    private void TriggerAreaStrike(ICombatTarget directTarget)
    {
        var strikes = context.AreaStrikes;
        if (strikes == null || strikes.Count == 0) return;

        foreach (var strike in strikes)
        {
            SpellAreaStrike.Spawn(strike, transform.position, context, currentDamage, directTarget);
        }
    }

    private void SpawnHitVfx()
    {
        if (defaultHitVfx != null)
        {
            Instantiate(defaultHitVfx, transform.position, Quaternion.LookRotation(-moveDirection));
        }
    }

    /// <summary>
    /// 투사체 소멸 처리 (연쇄가 끝났거나 사거리 도달)
    /// </summary>
    private void OnExpire(bool hitTarget)
    {
        // 잔영 노드: 소멸 위치에 속성 피해 잔상/장판 생성
        if (shadowVfxPrefab != null)
        {
            Instantiate(shadowVfxPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
