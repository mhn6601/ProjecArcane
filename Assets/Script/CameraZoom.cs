using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minRadius = 3.5f;
    [SerializeField] private float maxRadius = 8f;
    [SerializeField] private float smoothTime = 0.15f;
    [SerializeField] private bool ZoomAble = false;
    private CinemachineOrbitalFollow orbitalFollow;
    private CinemachineFollow follow;

    private float targetRadius;
    private float smoothVelocity;

    private void Awake()
    {
        
        cinemachineCamera.TryGetComponent<CinemachineOrbitalFollow>(out CinemachineOrbitalFollow OrbitalFollow);
        orbitalFollow = OrbitalFollow;

        if (orbitalFollow)
        {

            targetRadius = orbitalFollow.Radius;
        }
    }

    private void Update()
    {
        if (ZoomAble)
        {
            if (Player.Instance && Player.Instance.Input.InputEnabled)
            {
                float scroll = Mouse.current.scroll.ReadValue().y;
                if (orbitalFollow)
                {
                    if (scroll != 0)
                    {
                        targetRadius -= scroll * zoomSpeed;

                        targetRadius = Mathf.Clamp(
                            targetRadius,
                            minRadius,
                            maxRadius
                        );
                    }


                    orbitalFollow.Radius = Mathf.SmoothDamp(
                        orbitalFollow.Radius,
                        targetRadius,
                        ref smoothVelocity,
                        smoothTime
                    );
                }
            }
        }
        
        
    }
}