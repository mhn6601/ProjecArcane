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
    public Transform GetPrimaryPoint()
    {
        // LockOnPoint가 컴포넌트라면
        if (lockOnPoints != null && lockOnPoints.Count > 0 && lockOnPoints[0] != null)
            return lockOnPoints[0].transform;

        return transform;
    }
}