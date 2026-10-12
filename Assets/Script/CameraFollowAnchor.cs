using UnityEngine;

public class CameraFollowAnchor : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 15f;

    [Header("Vertical Smoothing (계단/단차 대응)")]
    [SerializeField] private float ySmoothTime = 0.12f;  // 클수록 부드럽고 느리게 따라옴
    [SerializeField] private float yMaxSpeed = 20f;      // 큰 낙하 시 카메라가 너무 뒤처지지 않게 상한

    private Transform player;
    private PlayerLockOn lockOn;
    private float currentY;
    private float yVelocity;

    private void Awake()
    {
        player = Player.Instance.transform;
        lockOn = Player.Instance.LockOn;
        SnapToPlayer();
    }

    private void LateUpdate()
    {
        // X/Z는 즉시 따라가고, Y만 SmoothDamp로 보간한다
        Vector3 p = player.position;
        currentY = Mathf.SmoothDamp(currentY, p.y, ref yVelocity, ySmoothTime, yMaxSpeed);
        transform.position = new Vector3(p.x, currentY, p.z);

        if (lockOn.IsLockedOn && lockOn.CurrentTarget != null)
        {
            // 캐릭터 회전과 무관하게, 항상 타겟 방향을 기준으로 카메라 위치 계산
            Vector3 dir = lockOn.CurrentTarget.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }
        }
        else
        {
            // 논록온: 실제 캐릭터 회전을 그대로 따라감
            transform.rotation = player.rotation;
        }
    }
    // 텔레포트, 리스폰, 씬 전환 직후 등 카메라가 순간이동해야 할 때 호출
    public void SnapToPlayer()
    {
        currentY = player.position.y;
        yVelocity = 0f;
        transform.position = player.position;
    }

}