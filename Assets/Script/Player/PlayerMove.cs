using System.Collections;
using Unity.Mathematics;
using UnityEngine;

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
        if (player.Status.IsLockOn)
        {
            if (isRun && moveDir.sqrMagnitude > 0.001f)
            {
                // 락온 + 달리기: 캐릭터가 이동 방향을 보게 함 (카메라는 CameraFollowAnchor가 별도로 타겟 고정)
                RotateTowardsDirection(moveDir);
            }
            else
            {
                // 락온 + 정지/걷기: 타겟을 계속 봄
                RotateTowardsTarget(player.LockOn.CurrentTarget.position);
            }
        }
        else
        {
            RotateTowardsDirection(moveDir);
        }

        // 이동 처리
        float currentSpeed = isRun ? player.Stats.MoveSpeed * player.Stats.RunSpeedMultiply : player.Stats.MoveSpeed;
        Vector3 horizontalMove = moveDir * currentSpeed;
        Vector3 verticalMove = new Vector3(0f, -8f, 0f);
        controller.CC.Move((horizontalMove + verticalMove) * Time.deltaTime);
    }
    

    // 카메라 회전을 고려하여 입력 방향을 월드 좌표계 방향으로 변환하는 연산 함수
    private Vector3 CalculateDirection(Vector3 input)
    {
        Vector3 dirForward;
        Vector3 dirRight;

        if (player.Status.IsLockOn &&!player.Status.IsRunning)
        {
            // 록온 중: 카메라가 아니라 캐릭터 자신의 정면(=타겟 방향) 기준
            dirForward = transform.forward;
            dirRight = transform.right;
        }
        else if(player.Status.IsLockOn && player.Status.IsRunning)
        {
            Vector3 forward=(player.LockOn.CurrentTarget.transform.position - transform.position).normalized;
            forward.y = 0f;
            dirForward = forward;
            Quaternion right = Quaternion.Euler(0f, 90f, 0f);
            dirRight = right*dirForward ;
        }
        else
        {
            if (playerFollowCamera == null) return Vector3.zero;

            dirForward = playerFollowCamera.forward;
            dirRight = playerFollowCamera.right;
        }

        dirForward.y = 0f;
        dirRight.y = 0f;

        dirForward.Normalize();
        dirRight.Normalize();
        Vector3 result = (dirForward * input.z) + (dirRight * input.x);
        if (result.magnitude > 1f)
        {
            result.Normalize();
        }
        return result;
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

    // 즉시회전
    public void RotateInstant(Vector3 worldDir)
    {
        if (worldDir.sqrMagnitude <= 0.001f) return;

        // 혹시 진행 중인 보간 회전(공격 스냅 등)이 있다면 중단
        if (attackRotateCoroutine != null)
        {
            StopCoroutine(attackRotateCoroutine);
            attackRotateCoroutine = null;
        }

        transform.rotation = Quaternion.LookRotation(worldDir);
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