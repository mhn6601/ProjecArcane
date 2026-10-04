using UnityEngine;

public enum MagicSocketType
{
    RightHand,      // 오른손 (주무기/지팡이)
    LeftHand,       // 왼손 (보조무기/아뮬렛)
    StaffTip,       // 지팡이 끝 (특수 발사 지점)
    Chest,          // 가슴/체스트 (방출형 스펠)
    CastPoint       // 캐스팅 전용 공중 소켓
}

public class MagicSocket : MonoBehaviour
{
    [Header("Socket Configuration")]
    [SerializeField] private MagicSocketType socketType;
    [SerializeField] private Vector3 localOffset = Vector3.zero;
    [SerializeField] private Vector3 localRotationOffset = Vector3.zero;

    public MagicSocketType SocketType => socketType;

    /// <summary>
    /// 실제 스펠/투사체가 생성될 월드 좌표 반환
    /// </summary>
    public Vector3 Position => transform.TransformPoint(localOffset);

    /// <summary>
    /// 스펠/투사체의 생성 회전값 반환 (오프셋 적용)
    /// </summary>
    public Quaternion Rotation => transform.rotation * Quaternion.Euler(localRotationOffset);

    /// <summary>
    /// Transform 자체 참조 필요 시
    /// </summary>
    public Transform SocketTransform => transform;

    private void OnDrawGizmosSelected()
    {
        // 에디터 상에서 소켓 발사 위치 시각화
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(Position, 0.05f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Position, Rotation * Vector3.forward * 0.3f);
    }
}