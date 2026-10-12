using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerJump : MonoBehaviour
{
    Player player;
    [Header("GroundCheck")]
    [SerializeField]
    private LayerMask groundLayer;
    private float gravity =-9.81f;
    private float groundedGravity =-8f;
    public float GroundedGravity => groundedGravity;
    [SerializeField]private Vector3 jumpVelocity;
    [SerializeField] private float groundCheckRadius = 0.25f;   // 컨트롤러 radius보다 약간 작게
    [SerializeField] private float groundCheckExtraDistance = 0.3f; // 계단 한 칸 높이 이상으로
    [SerializeField] private float groundedGraceTime = 0.12f;   // 지면을 놓쳐도 이 시간 동안은 접지로 유지
    private float lastGroundedTime = -999f;

    private PlayerController controller;

    private PlayerInputAction inputActions;
    [SerializeField] private Transform playerFollowCamera;
    private void Awake()
    {
        player = Player.Instance;
        inputActions = new PlayerInputAction();
        controller = player.Controller;
        
        
    }
    private void Start()
    {
        IsGroundedInitialize();

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
     void FixedUpdate()
    {
        ApplyGravity();
        //Debug.Log(jumpVelocity.y);
    }
    public void ExecuteJump(Vector3 rawInput, bool isRun)
    {
        NotifyLeftGround();
        // 1. 수평 이동 벡터 계산 (X, Z) - 속도 적용
        float currentSpeed = isRun ? player.Stats.MoveSpeed*player.Stats.RunSpeedMultiply : player.Stats.MoveSpeed;
        Vector3 horizontalMove = rawInput * currentSpeed;

        // 2. 수직 이동 벡터 계산 (Y) - 중력/점프 속도 적용
        Vector3 verticalMove = new Vector3(0f, GetJumpVelocity(), 0f);

        // 3. 최종 통합 이동 벡터 전달
        Vector3 finalVelocity = horizontalMove + verticalMove;
        controller.CC.Move(finalVelocity * Time.deltaTime);
    }
    private bool CheckGroundHit()
    {
        // 구체가 지면에 파묻혀 시작하지 않도록 반지름만큼 위에서 시작
        Vector3 origin = GetRayOrigin() + Vector3.up * groundCheckRadius;
        float distance = groundCheckExtraDistance + 0.1f; // 0.1f = skinWidth 등 시작점 여유분

        return Physics.SphereCast(
            origin, groundCheckRadius, Vector3.down,
            out _, distance, groundLayer,
            QueryTriggerInteraction.Ignore);
    }
    public bool IsGround()
    {
        if (CheckGroundHit())
        {
            lastGroundedTime = Time.time;
            return true;
        }

        // 한두 프레임 지면을 놓친 경우 접지로 간주 (계단 오를 때 컨트롤 뺏기는 현상 방지)
        return Time.time - lastGroundedTime <= groundedGraceTime;
    }

    // 점프/넉백 등으로 의도적으로 공중에 뜰 때 호출해서 유예가 적용되지 않게 한다
    public void NotifyLeftGround()
    {
        lastGroundedTime = -999f;
    }
    private void IsGroundedInitialize()
    {
        if (CheckGroundHit())
        {
            player.Status.RemoveFlag(CharacterActionFlags.CanJump);
            player.Status.RemoveFlag(CharacterActionFlags.CanRoll);
        }
        else
        {
            player.Status.AddFlag(CharacterActionFlags.CanRoll);
            player.Status.AddFlag(CharacterActionFlags.CanJump);
        }
    }
    public void SetJumpVelocity()
    {
        jumpVelocity.y = Mathf.Sqrt(player.Stats.Jumphight * -2f * gravity);
    }
    public float GetJumpVelocity()
    {
        return jumpVelocity.y;
    }
    public void ApplyGravity()
    {
        if (IsGround() && jumpVelocity.y <= 0)
        {
            jumpVelocity.y = groundedGravity;
        }
        else{
            jumpVelocity.y += gravity * Time.fixedDeltaTime;
        }
    }
    public Vector3 CalculateDirection(Vector3 input)
    {
        return player.Move.GetWorldDirection(input);
    }
    private Vector3 GetRayOrigin()
    {
        if (controller != null && controller.CC != null)
        {
            return transform.position + new Vector3(0f, controller.CC.center.y - (controller.CC.height * 0.5f) + controller.CC.skinWidth, 0f);
        }

        // 에디터 실행 전 참조 연산용 기본 위치
        return transform.position;
    }
    private void OnDrawGizmos()
    {
        Vector3 start = GetRayOrigin() + Vector3.up * groundCheckRadius;
        float distance = groundCheckExtraDistance + 0.1f;

        Gizmos.color = CheckGroundHit() ? Color.green : Color.red;
        Gizmos.DrawWireSphere(start, groundCheckRadius);
        Gizmos.DrawWireSphere(start + Vector3.down * distance, groundCheckRadius);
    }
}
