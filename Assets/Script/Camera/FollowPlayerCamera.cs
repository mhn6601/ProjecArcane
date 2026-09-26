using Unity.Cinemachine;
using UnityEngine;

public class FollowPlayerCamera : Singleton<FollowPlayerCamera>
{
    public Transform followCameraTransform;
    public CinemachineCamera cinemachineCamera;
    protected override void Awake()
    {
        base.Awake();
        followCameraTransform = gameObject.transform;
        cinemachineCamera = GetComponent<CinemachineCamera>();
    }
}
