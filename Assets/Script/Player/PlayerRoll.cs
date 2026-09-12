using UnityEngine;

public class PlayerRoll : MonoBehaviour
{
    Player player;
    [SerializeField] private Transform playerFollowCamera;
    private void Awake()
    {
        player = Player.Instance;

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
    public void ExcuteRoll()
    {

       // 1.수직 이동 벡터 계산(Y) - 중력 / 점프 속도 적용
        Vector3 verticalMove = new Vector3(0f, player.Jump.GetJumpVelocity(), 0f);

        player.Controller.CC.Move(verticalMove * Time.deltaTime);

    }
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
    public void RotateTargetInstant(Vector3 input)
    {
        // Y축 높이(수직 요소) 제거하여 평면 이동 방향만 추출
        Vector3 rollDirection = new Vector3(input.x, 0f, input.z);

        // 입력값이 유효한 경우에만 (입력이 거의 0에 가까우면 회전하지 않음)
        if (rollDirection.sqrMagnitude > 0.001f)
        {
            // 입력 방향을 바라보는 쿼터니언 회전값 생성
            Quaternion targetRotation = Quaternion.LookRotation(rollDirection);

            // 부드러운 보간(Slerp) 없이 즉시 회전 적용
            transform.rotation = targetRotation;
        }
    }
}
