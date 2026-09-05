using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerJump : MonoBehaviour
{
    [Header("GroundCheck")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float rayDistance=0.2f;
    private float gravity =-9.81f;
    private float groundedGravity =-8f;
    public float GroundedGravity => groundedGravity;
    [SerializeField]private Vector3 jumpVelocity;

    [SerializeField]
    private float jumpHeight = 0f;

    private PlayerController controller;

    private PlayerInputAction inputActions;
    [SerializeField] private Transform playerFollowCamera;
    private void Awake()
    {
        inputActions = new PlayerInputAction();
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
    private void Start()
    {
        IsGroundedInitialize();
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
        // 1. 수평 이동 벡터 계산 (X, Z) - 속도 적용
        float currentSpeed = isRun ? Player.Instance.RunSpeed : Player.Instance.MoveSpeed;
        Vector3 horizontalMove = rawInput * currentSpeed;

        // 2. 수직 이동 벡터 계산 (Y) - 중력/점프 속도 적용
        Vector3 verticalMove = new Vector3(0f, GetJumpVelocity(), 0f);

        // 3. 최종 통합 이동 벡터 전달
        Vector3 finalVelocity = horizontalMove + verticalMove;
        controller.CC.Move(finalVelocity * Time.deltaTime);
    }
    public bool IsGround()
    {
        Vector3 ray = GetRayOrigin();        
        return Physics.Raycast(ray, Vector3.down, rayDistance, groundLayer);
    }
    private void IsGroundedInitialize()
    {
        Vector3 ray = GetRayOrigin();
        if(Physics.Raycast(ray, Vector3.down, rayDistance, groundLayer))
        {
            controller.Status.RemoveFlag(CharacterActionFlags.CanJump);
        }
        else
        {
            controller.Status.AddFlag(CharacterActionFlags.CanJump);
        }
    }
    public void SetJumpVelocity()
    {
        jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
        if (playerFollowCamera == null) return Vector3.zero;

        var camForward = playerFollowCamera.forward;
        var camRight = playerFollowCamera.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        return (camForward * input.z) + (camRight * input.x);
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
        Vector3 rayOrigin = GetRayOrigin();

        // 지면에 닿았을 때는 초록색, 공중에 있을 때는 빨간색 표시
        Gizmos.color = IsGround() ? Color.green : Color.red;

        // 시작점에서 아래 방향으로 rayDistance만큼 선 그리기
        Gizmos.DrawRay(rayOrigin, Vector3.down * rayDistance);

        // Ray의 끝 지점에 작은 구체 표시
        Gizmos.DrawSphere(rayOrigin + (Vector3.down * rayDistance), 0.02f);
    }
}
