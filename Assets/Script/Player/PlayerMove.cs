using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 15f;

    private Vector3 moveVelocity;
    
    private Vector3 cachedMoveDirection; // 계산된 최종 방향 벡터 저장용

    private PlayerController controller;

    [SerializeField] private Transform playerFollowCamera;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

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
    public void ExecuteMove(Vector3 rawInput, bool isRun)
    {
        if (rawInput.sqrMagnitude <= 0.001f && !controller.Status.IsLockOn) return;

        Vector3 moveDir = CalculateDirection(rawInput);
        // [회전 분기]
        if (Player.Instance != null && controller.Status.IsLockOn)
        {
            // 락온 상태: 이동 방향이 아니라 '타겟(적)'을 바라보도록 회전
            RotateTowardsTarget(Player.Instance.lockOnTarget.position);
        }
        else
        {
            // 논락온 상태: 자신이 이동하는 방향을 바라보도록 회전
            RotateTowardsDirection(moveDir);
            //Debug.Log("논락온 상태 회전 처리");
        }

        // 이동 처리
        float currentSpeed = isRun ? Player.Instance.PlayerStats.RunSpeed : Player.Instance.PlayerStats.MoveSpeed;
        moveDir.y = -8f;
        controller.CC.Move(moveDir * currentSpeed * Time.deltaTime);
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
    private void RotateTowardsTarget(Vector3 targetPos)
    {
        Vector3 dirToTarget = targetPos - transform.position;
        dirToTarget.y = 0f; // Y축 고정

        if (dirToTarget.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dirToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

}