// ScarecrowBrain.cs
using UnityEngine;

public class TestDummyBrain : EnemyBrain
{
    [SerializeField] private float attackInterval = 3f;
    [SerializeField] private int patternIndex = 0;

    private EnemyAttack attack;
    private float timer;

    protected override void Awake()
    {
        base.Awake();
        attack = GetComponent<EnemyAttack>();
    }

    protected override void Think()
    {
        if (!controller.IsActionIdle) return;   // 공격 중에는 대기 시간을 세지 않음

        timer += Time.deltaTime;
        if (timer >= attackInterval && attack.TryStartPattern(patternIndex))
            timer = 0f;
    }
}