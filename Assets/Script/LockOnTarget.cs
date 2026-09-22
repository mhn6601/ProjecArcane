using System.Collections.Generic;
using UnityEngine;

public class LockOnTarget : MonoBehaviour
{
    [SerializeField]
    private List<LockOnPoint> lockOnPoints = new();
    [SerializeField]
    private EnemyLockOnPointGenerator enemyLockOnPointGenerator;
    public IReadOnlyList<LockOnPoint> LockOnPoints =>
        lockOnPoints;
    public Vector3 position;
    private void Start()
    {
        position = transform.position;
        lockOnPoints = enemyLockOnPointGenerator.lockOnPointList;
    }
    public Transform GetTargetPoint()
    {
        if (lockOnPoints == null ||
            lockOnPoints.Count == 0)
        {
            return transform;
        }

        foreach (LockOnPoint point in lockOnPoints)
        {
            if (point != null &&
                point.IsAvailable)
            {
                return point.transform;
            }
        }

        return null;
    }
}