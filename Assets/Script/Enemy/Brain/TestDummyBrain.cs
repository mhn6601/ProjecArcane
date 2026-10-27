// TestDummyBrain.cs
using UnityEngine;

public class TestDummyBrain : EnemyBrain
{
    [SerializeField] private float attackInterval = 3f;
    [SerializeField] private int patternIndex = 0;

    private Enemy enemy;
    private EnemyAttack attack;
    private float timer;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponent<Enemy>();
        attack = GetComponent<EnemyAttack>();
    }

    private void OnEnable()
    {
        if (enemy != null)
            enemy.OnRevived += ResetTimer;
    }

    private void OnDisable()
    {
        if (enemy != null)
            enemy.OnRevived -= ResetTimer;
    }

    protected override void Think()
    {
        if (enemy.isDead) return;                // 사망 중에는 판단/타이머 진행 안 함
        if (!controller.IsActionIdle) return;    // 공격 중에는 대기 시간을 세지 않음

        timer += Time.deltaTime;
        if (timer >= attackInterval && attack.TryStartPattern(patternIndex))
            timer = 0f;
    }

    /// <summary>부활 직후 즉시 공격하지 않도록 대기 시간을 처음부터 다시 센다.</summary>
    private void ResetTimer()
    {
        timer = 0f;
    }
}