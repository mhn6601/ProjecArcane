using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Attack Set")]
public class EnemyAttackSet : ScriptableObject
{
    public List<EnemyAttackPattern> patterns = new();
}