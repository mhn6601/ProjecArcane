using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Combat/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHp = 100f;
    public float moveSpeed = 3.5f;
    public float detectRange = 10f;  // 감지 범위
    public float attackRange = 2f;   // 공격 범위

    // 몬스터 전용 데이터 (필요 시 공격 데이터 배열 등)
}
