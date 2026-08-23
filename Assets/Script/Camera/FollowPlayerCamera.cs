using UnityEngine;

public class FollowPlayerCamera : Singleton<FollowPlayerCamera>
{
    public Transform followCameraTransform;
    protected override void Awake()
    {
        base.Awake();
        followCameraTransform = gameObject.transform;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
