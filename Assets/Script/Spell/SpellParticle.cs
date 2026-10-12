using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SpellParticle : MonoBehaviour
{
    [Header("Impact VFX")]
    [SerializeField] private GameObject defaultHitVfx;
    [SerializeField] private GameObject shadowVfxPrefab; // 잔영 노드용 장판 이펙트

    private SpellCastContext context;
    private Vector3 moveDirection;
    private float currentLifetime = 0f;
    private float maxLifetime = 5f; // 사거리 및 연장 노드 영향
    private bool isInitialized = false;

    // 타격된 대상 중복 피격 방지
    private System.Collections.Generic.HashSet<Collider> hitColliders = new System.Collections.Generic.HashSet<Collider>();

    /// <summary>
    /// FSM 시전 상태에서 Instantiate 직후 호출하여 Context 정보 주입
    /// </summary>
    public void Initialize(SpellCastContext context)
    {
        this.context = context;
        this.moveDirection = context.CastDirection;
        this.isInitialized = true;

        // 1. 크기 배율 적용 (확장 노드)
        transform.localScale *= context.ScaleMultiplier;

        // 2. 사거리/수명 연장 적용 (연장 노드)
        maxLifetime *= (context.ScaleMultiplier > 1f) ? 1.5f : 1f; // 예시 수명 연장 연산

        // 3. 트리거 레이어 설정
        GetComponent<Collider>().isTrigger = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        // 1. 이동 처리 (추적/유도 노드 연산)
        if (context.HasHoming && context.TargetTransform != null)
        {
            Vector3 targetDir = (context.TargetTransform.position - transform.position).normalized;
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
        // 시전 자 및 중복 피격 대상 방지
        if (other.gameObject == context.Caster || hitColliders.Contains(other)) return;

        // 1. ICombatTarget 인터페이스 탐색 (자식 콜라이더 피격 대응을 위해 GetComponentInParent 권장)
        if (!other.TryGetComponent<ICombatTarget>(out var target))
        {
            target = other.GetComponentInParent<ICombatTarget>();
        }

        if (target != null && !target.IsDead)
        {
            hitColliders.Add(other);

            // 2. 공격 데이터(AttackData) 생성 및 히트 파트(HitPart) 지정
            // 필요에 따라 AttackData 생성자 또는 구조체 초기화
            AttackData attackData = new AttackData
            {
                damage = context.FinalDamage,
                element = context.Element,
                // 기타 AttackData 필드 바인딩
            };

            // 3. ICombatTarget.ResolveHit 호출
            target.ResolveHit(attackData,context.Caster.transform,transform, HitPart.Body);

            // 4. 경직(Stagger) 수치가 정의되어 있다면 함께 전달
            target.ApplyStagger(10f); // 예시 경직 수치

            // 히트 성공 후 소멸 처리
            OnExpire(true);
        }
    }

    /// <summary>
    /// 투사체 소멸 처리 (타격 또는 사거리 도달)
    /// </summary>
    private void OnExpire(bool hitTarget)
    {
        // 잔영 노드: 소멸 위치에 속성 피해 잔상/장판 생성
        if (shadowVfxPrefab != null)
        {
            Instantiate(shadowVfxPrefab, transform.position, Quaternion.identity);
        }

        // 히트 이펙트 스폰
        if (hitTarget && defaultHitVfx != null)
        {
            Instantiate(defaultHitVfx, transform.position, Quaternion.LookRotation(-moveDirection));
        }

        Destroy(gameObject);
    }
}