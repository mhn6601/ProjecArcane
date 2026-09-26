using UnityEngine;

public class LockOnLookAtTarget : MonoBehaviour
{
    [SerializeField] private float heightOffset = 1.2f;
    [Header("0=적만, 1=플레이어만")]
    [Range(0f, 1f)]

    [SerializeField] private float playerWeight = 0.4f; 

    private Transform _player;
    private Transform _target;

    public void SetTargets(Transform player, Transform target)
    {
        _player = player;
        _target = target;
    }

    private void LateUpdate()
    {
        if (_player == null || _target == null) return;

        Vector3 midPoint = Vector3.Lerp(_target.position, _player.position, playerWeight);
        midPoint.y += heightOffset;
        transform.position = midPoint;
    }
}