using System;

[Serializable]
public class EnemyAttackStepData : AttackData
{
    public float crossFade = 0.1f;
    public float delayAfter = 0f;       // 이 스텝 종료 후 다음 스텝까지 대기 시간 (0이면 즉시 이어짐)
    public bool rotateToTarget = false; // 스텝 시작 시 타겟 방향으로 회전할지 (추후 사용)
}