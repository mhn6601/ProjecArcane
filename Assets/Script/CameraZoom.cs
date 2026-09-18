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

    private CinemachineOrbitalFollow orbitalFollow;

    private float targetRadius;
    private float smoothVelocity;

    private void Awake()
    {
        orbitalFollow =
            cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();

        targetRadius = orbitalFollow.Radius;
    }

    private void Update()
    {
        if (Player.Instance&&Player.Instance.Input.InputEnabled)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

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