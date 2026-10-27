using System.Collections;
using UnityEngine;

/// <summary>
/// 훈련용 허수아비 전용: 사망 후 자동 부활 + 일정 시간 피격이 없으면 체력 전부 회복.
/// 이 컴포넌트가 없는 몬스터는 사망 시 그대로 영구 사망한다.
/// </summary>
[RequireComponent(typeof(Enemy))]
public class EnemyRevive : MonoBehaviour
{
    [Header("Revive")]
    [SerializeField] private float reviveDelay = 5f;

    [Header("Idle Heal (0 이하면 비활성)")]
    [SerializeField] private float idleHealDelay = 5f;

    private Enemy enemy;
    private EnemyResourceSystem resource;
    private float lastHealth;
    private float lastDamagedTime;
    private Coroutine reviveRoutine;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        resource = enemy.ResourceSystem;
        lastHealth = resource.CurrentHealth;

        resource.OnDeath += HandleDeath;
        resource.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDestroy()
    {
        if (resource == null) return;
        resource.OnDeath -= HandleDeath;
        resource.OnHealthChanged -= HandleHealthChanged;
    }

    private void Update()
    {
        if (idleHealDelay <= 0f || enemy.isDead) return;

        bool damaged = resource.CurrentHealth < resource.MaxHealth;
        if (damaged && Time.time >= lastDamagedTime + idleHealDelay)
            resource.HealHP(resource.MaxHealth);
    }

    private void HandleHealthChanged(float current, float max)
    {
        if (current < lastHealth)
            lastDamagedTime = Time.time;   // 피해를 받은 시점 기록
        lastHealth = current;
    }

    private void HandleDeath()
    {
        if (reviveRoutine != null) StopCoroutine(reviveRoutine);
        reviveRoutine = StartCoroutine(ReviveRoutine());
    }

    private IEnumerator ReviveRoutine()
    {
        yield return new WaitForSeconds(reviveDelay);
        reviveRoutine = null;
        enemy.Revive();
    }
}