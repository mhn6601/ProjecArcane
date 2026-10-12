// EnemyAttackPattern.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Attack Pattern")]
public class EnemyAttackPattern : ScriptableObject
{
    [Header("Steps (연속 공격)")]
    public List<EnemyAttackStepData> steps = new();

    [Header("Selection")]
    public float weight = 1f;       // 랜덤 선택 가중치
    public float minRange = 0f;     // 이 패턴이 사용 가능한 최소 거리
    public float maxRange = 3f;     // 이 패턴이 사용 가능한 최대 거리
    public float cooldown = 0f;     // 패턴 종료 후 재사용 대기 시간

    public bool InRange(float distance) => distance >= minRange && distance <= maxRange;
}