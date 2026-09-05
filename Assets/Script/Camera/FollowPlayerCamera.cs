using UnityEngine;

public class FollowPlayerCamera : Singleton<FollowPlayerCamera>
{
    public Transform followCameraTransform;
    protected override void Awake()
    {
        base.Awake();
        followCameraTransform = gameObject.transform;
    }
}
