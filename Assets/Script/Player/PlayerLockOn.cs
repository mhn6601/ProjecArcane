using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerLockOn : MonoBehaviour
{
    Player player;
    [SerializeField] private float lockOnRadius = 15f;
    [SerializeField] private float maxAngle = 60f; // 카메라 정면 기준 허용 각도
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private CinemachineCamera lockOnCamera;
    [SerializeField] private CinemachineCamera freeLookCamera;
    [SerializeField] private LockOnLookAtTarget lookAtRig;

    public Transform CurrentTarget { get; private set; }
    public Transform CurrentLockOnPoint { get; private set; }
    public Transform AimPoint => CurrentLockOnPoint != null ? CurrentLockOnPoint : CurrentTarget;
    public bool IsLockedOn => CurrentTarget != null;

    private Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
        player = Player.Instance;
       
    }
    private void Start()
    {
        freeLookCamera = FollowPlayerCamera.Instance.cinemachineCamera;
    }
    private void OnEnable()
    {
        player.Input.OnLockOnClicked+= TryLockOn;
    }
    private void OnDisable()
    {
        player.Input.OnLockOnClicked -= TryLockOn;
    }
    public void TryLockOn()
    {
        if (IsLockedOn) { ReleaseLockOn(); return; }

        Transform best = FindBestTarget();
        if (best == null) return;

        CurrentTarget = best; 
        CurrentLockOnPoint = ResolveLockOnPoint(best);
        lookAtRig.SetTargets(Player.Instance.transform, CurrentTarget);
        lockOnCamera.Priority.Value = freeLookCamera.Priority.Value + 10;
        // ActionFSM/MovementFSM에 록온 상태 플래그 전달 (전략 이동 등으로 전환)
        Player.Instance.Status.AddFlag(CharacterActionFlags.isLockOn);
    }

    public void ReleaseLockOn()
    {
        CurrentTarget = null;
        CurrentLockOnPoint = null;
        lockOnCamera.Priority.Value = freeLookCamera.Priority.Value - 10;
        SyncFreeLookToLockOnCamera();
        Player.Instance.Status.RemoveFlag(CharacterActionFlags.isLockOn);
    }
    private Transform ResolveLockOnPoint(Transform target)
    {
        LockOnTarget lockOnTarget = target.GetComponentInParent<LockOnTarget>();
        Transform point = lockOnTarget != null ? lockOnTarget.GetPrimaryPoint() : null;
        return point != null ? point : target;
    }
    private void SyncFreeLookToLockOnCamera()
    {
        var orbital = freeLookCamera.GetComponent<CinemachineOrbitalFollow>();
        if (orbital == null) return;

        Vector3 pivot = Player.Instance.transform.position; // Follow 기준점
        Vector3 dir = pivot- lockOnCamera.transform.position;

        // 수평 각도(Yaw) 계산 — 월드 기준
        float yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        // 수직 각도(Pitch) 계산
        float horizontalDist = new Vector2(dir.x, dir.z).magnitude;
        float pitch = Mathf.Atan2(dir.y, horizontalDist) * Mathf.Rad2Deg+50;
        //Debug.Log(pitch);
        orbital.HorizontalAxis.Value = yaw;
        orbital.VerticalAxis.Value = pitch;
    }
    private Transform FindBestTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, lockOnRadius, enemyLayer);
        Transform best = null;
        float bestScore = float.MaxValue;

        foreach (var hit in hits)
        {
            Vector3 dirToTarget = hit.transform.position - _mainCam.transform.position;
            float angle = Vector3.Angle(_mainCam.transform.forward, dirToTarget);
            if (angle > maxAngle) continue;

            // 장애물 차단 체크 (벽 뒤 적 방지)
            if (Physics.Linecast(_mainCam.transform.position, hit.transform.position, obstacleLayer))
                continue;

            // 화면 중앙에 가까울수록, 가까울수록 우선순위 높음
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            float score = angle * 1.0f + distance * 0.5f;

            if (score < bestScore)
            {
                bestScore = score;
                best = hit.transform;
            }
        }
        return best;
    }

    private void LateUpdate()
    {
        // 타겟이 죽거나 사거리 밖으로 나가면 자동 해제
        if (IsLockedOn)
        {
            // 1. 빠른 탈출: 간단한 Bool/인스턴스 체크를 사거리 계산보다 먼저 실행
            if (CurrentTarget == null || !CurrentTarget.gameObject.activeSelf)
            {
                ReleaseLockOn();
            }
            else
            {
                // 2. 제곱 거리 비교: Vector3.Distance(제곱근 연산) 대신 sqrMagnitude 사용
                //    (lockOnRadius * 1.3f)의 제곱을 미리 캐싱해두면 훨씬 좋습니다.
                float maxDistanceSqr = (lockOnRadius * 1.3f) * (lockOnRadius * 1.3f);

                // Transform.position 캐싱 및 SqrMagnitude 연산
                Vector3 diff = CurrentTarget.position - transform.position;
                if (diff.sqrMagnitude > maxDistanceSqr)
                {
                    ReleaseLockOn();
                }
            }
        }
    }
}