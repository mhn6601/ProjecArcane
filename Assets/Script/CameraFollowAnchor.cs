using UnityEngine;

public class CameraFollowAnchor : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 15f;
    private Transform player;
    private PlayerLockOn lockOn;

    private void Awake()
    {
        player = Player.Instance.transform;
        lockOn = Player.Instance.LockOn;
    }

    private void LateUpdate()
    {
        transform.position = player.position;

        if (lockOn.IsLockedOn && lockOn.CurrentTarget != null)
        {// 캐릭터 회전과 무관하게, 항상 타겟 방향을 기준으로 카메라 위치 계산
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
}