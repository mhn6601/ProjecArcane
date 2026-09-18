using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    Player player;
    [SerializeField] private float rotateSpeed = 15f;
    [SerializeField] private float attackRotateDuration = 0.15f;

    private PlayerController controller;
    private Coroutine attackRotateCoroutine;

    [SerializeField] private Transform playerFollowCamera;

    public float requiredRunStaminaPerSecond = 3f;

    private void Awake()
    {
        player = Player.Instance;
        controller = player.Controller;

        
    }
    private void Start()
    {
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
        if (rawInput.sqrMagnitude <= 0.001f && !player.Status.IsLockOn) return;

        Vector3 moveDir = CalculateDirection(rawInput);
        // [회전 분기]
        if ( player.Status.IsLockOn)
        {
            // 락온 상태: 이동 방향이 아니라 '타겟(적)'을 바라보도록 회전
            RotateTowardsTarget(player.lockOnTarget.position);
        }
        else
        {
            // 논락온 상태: 자신이 이동하는 방향을 바라보도록 회전
            RotateTowardsDirection(moveDir);
            //Debug.Log("논락온 상태 회전 처리");
        }

        // 이동 처리
        float currentSpeed = isRun ? player.Stats.MoveSpeed*player.Stats.RunSpeedMultiply : player.Stats.MoveSpeed;
        moveDir.y = -8f;
        controller.CC.Move(moveDir * currentSpeed * Time.deltaTime);
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

    public void RotateTowardsDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }
    public Vector3 GetWorldDirection(Vector3 rawInput)
    {
        return CalculateDirection(rawInput);
    }
    // 공격 시작 시 한 번만 호출되는 1회성 보간 회전 (Slerp)
    public void RotateSnap(Vector3 worldDir)
    {
        if (worldDir.sqrMagnitude <= 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(worldDir);
        StartRotateCoroutine(targetRotation);
    }

    public void RotateSnapTowardsTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude <= 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        StartRotateCoroutine(targetRotation);
    }

    private void StartRotateCoroutine(Quaternion targetRotation)
    {
        if (attackRotateCoroutine != null)
        {
            StopCoroutine(attackRotateCoroutine);
        }
        attackRotateCoroutine = StartCoroutine(RotateSnapCoroutine(targetRotation));
    }
    private IEnumerator RotateSnapCoroutine(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float elapsed = 0f;

        while (elapsed < attackRotateDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / attackRotateDuration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
        attackRotateCoroutine = null;
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