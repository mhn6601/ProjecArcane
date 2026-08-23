using UnityEngine;
public class MainCamera : Singleton<MainCamera>
{
    public Transform mainCameraTransform;
    protected override void Awake()
    {
        base.Awake();
        mainCameraTransform=gameObject.transform;
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
