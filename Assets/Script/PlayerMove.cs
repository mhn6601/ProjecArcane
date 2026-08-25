using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;

    private Vector3 moveVelocity;
    
    private Vector3 cachedMoveDirection; // 계산된 최종 방향 벡터 저장용

    private PlayerController cc;

    [SerializeField] private Transform playerFollowCamera;

    private void Awake()
    {
        cc = GetComponent<PlayerController>();

        if (playerFollowCamera == null || playerFollowCamera != FollowPlayerCamera.Instance.transform)
        {
            if (FollowPlayerCamera.Instance != null)
            {
                playerFollowCamera = FollowPlayerCamera.Instance.transform;
            }
            else
            {
                if (Camera.main != null && FollowPlayerCamera.Instance == null)
                {
                    Debug.LogError("플레이어 팔로우 카메라 설정 오류 팔로우카메라 없음\nplayerFollowCamera=" + playerFollowCamera + "\nFollowPlayerCamera.Instance=" + FollowPlayerCamera.Instance);
                    playerFollowCamera = Camera.main.transform;
                }
                else
                {
                    playerFollowCamera = FindAnyObjectByType<Camera>().transform;
                    Debug.LogError("플레이어 팔로우 카메라 설정 오류- 팔로우카메라, 메인카메라 없음");
                }
            }
        }
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        
    }

    private void FixedUpdate()
    {
        // 입력이 있을 때만 실시간 카메라 정면 기준 계산 (카메라 회전에 따른 이동 방향 반영)
        if (cc.moveInput != Vector3.zero)
        {
            cachedMoveDirection = CalculateDirection(cc.moveInput);
        }

        // 락온 여부에 따른 회전 분기
        if (false)
        {
            // Step 1: 락온 구현 시 대상 조준 회전 로직 연동[cite: 1]
        }
        else
        {
            RotateTowardsDirection(cachedMoveDirection);
        }
        // 이동 처리
        moveVelocity = cachedMoveDirection * moveSpeed;
        moveVelocity.y = cc.GetJumpVelocity().y; // 점프 속도 적용


        cc.CC.Move(moveVelocity * Time.fixedDeltaTime);
    }

    public void ResetCache()
    {
        cachedMoveDirection = Vector3.zero;
    }

    // 카메라 회전을 고려하여 입력 방향을 월드 좌표계 방향으로 변환하는 연산 함수
    private Vector3 CalculateDirection(Vector3 input)
    {
        if (playerFollowCamera == null) return Vector3.zero;

        var camForward = playerFollowCamera.forward;
        var camRight = playerFollowCamera.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        return (camForward * input.z) + (camRight * input.x);
    }

    private void RotateTowardsDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    
}