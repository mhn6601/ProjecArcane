using Unity.Cinemachine;
using UnityEngine;

public class LockOnCamera : Singleton<LockOnCamera>
{
    public Transform lockOnCamera;
    public CinemachineCamera cinemachineCamera;
    protected override void Awake()
    {
        base.Awake();
        lockOnCamera = gameObject.transform;
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }
}
