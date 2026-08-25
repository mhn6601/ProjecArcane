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
    private Vector3 jumpVelocity;

    [SerializeField]
    private float jumpHeight = 0f;

    private PlayerController cc;

    private PlayerInputAction inputActions;
    private void Awake()
    {
        inputActions = new PlayerInputAction();
        cc = GetComponent<PlayerController>();
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    public void Jump()
    {
        if (IsGround())
        {
            SetJumpVelocityY();
        }
    }
    public bool IsGround()
    {
        Vector3 ray = GetRayOrigin();
        return Physics.Raycast(ray, Vector3.down, rayDistance, groundLayer);
    }
    public void SetJumpVelocityY()
    {
        jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
    public Vector3 GetJumpVelocity()
    {
        return jumpVelocity;
    }
    private void ApplyGravity()
    {
        if (IsGround() && jumpVelocity.y < 0)
        {
            jumpVelocity.y = groundedGravity;
        }
        else{
            jumpVelocity.y += gravity * Time.fixedDeltaTime;
        }
    }
    private Vector3 GetRayOrigin()
    {
        if (cc != null && cc.CC != null)
        {
            return transform.position + new Vector3(0f, cc.CC.center.y - (cc.CC.height * 0.5f) + cc.CC.skinWidth, 0f);
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
