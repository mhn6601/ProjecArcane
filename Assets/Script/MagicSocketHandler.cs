using System.Collections.Generic;
using UnityEngine;

public class MagicSocketHandler : MonoBehaviour
{
    [SerializeField] private List<MagicSocket> sockets = new List<MagicSocket>();
    private Dictionary<MagicSocketType, MagicSocket> socketDict = new Dictionary<MagicSocketType, MagicSocket>();

    private void Awake()
    {
        // 씬 내/자식 오브젝트의 MagicSocket들을 딕셔너리로 캐싱
        RefreshSockets();
    }

    /// <summary>
    /// 자식 오브젝트의 모든 MagicSocket을 탐색하여 딕셔너리에 캐싱
    /// </summary>
    public void RefreshSockets()
    {
        socketDict.Clear();
        MagicSocket[] foundSockets = GetComponentsInChildren<MagicSocket>(true);
        foreach (var socket in foundSockets)
        {
            if (!socketDict.ContainsKey(socket.SocketType))
            {
                socketDict.Add(socket.SocketType, socket);
            }
            else
            {
                Debug.LogWarning($"[MagicSocketHandler] 중복된 소켓 타입이 존재합니다: {socket.SocketType}");
            }
        }
    }

    /// <summary>
    /// [추가/복원된 함수] 지정된 타입의 MagicSocket 컴포넌트 참조 반환
    /// </summary>
    public MagicSocket GetSocket(MagicSocketType socketType)
    {
        if (socketDict.TryGetValue(socketType, out var socket))
        {
            return socket;
        }

        Debug.LogWarning($"[MagicSocketHandler] {socketType} 소켓을 찾지 못했습니다.");
        return null;
    }

    /// <summary>
    /// [추가/복원된 함수] 지정된 타입 소켓의 Transform 좌표를 안전하게 반환
    /// </summary>
    public Transform GetSocketTransform(MagicSocketType socketType)
    {
        MagicSocket socket = GetSocket(socketType);
        return socket != null ? socket.SocketTransform : transform;
    }

    /// <summary>
    /// 지정된 소켓 타입 위치에서 마법/투사체 인스턴스화 및 Context 전달
    /// </summary>
    public GameObject CastSpell(GameObject spellPrefab, MagicSocketType targetSocketType, SpellCastContext context)
    {
        if (spellPrefab == null)
        {
            Debug.LogError("[MagicSocketHandler] 생성할 spellPrefab이 null입니다.");
            return null;
        }

        MagicSocket socket = GetSocket(targetSocketType);

        Vector3 spawnPos = socket != null ? socket.Position : transform.position;
        Quaternion spawnRot = socket != null ? socket.Rotation : transform.rotation;
        // 분열 발사: 소켓 회전 대신 컨텍스트의 발사 방향을 따름
        if (context.UseCastDirectionRotation && context.CastDirection.sqrMagnitude > 0.0001f)
            spawnRot = Quaternion.LookRotation(context.CastDirection);
        // 발사체 생성
        GameObject spellInstance = Instantiate(spellPrefab, spawnPos, spawnRot);

        // SpellCastContext 전달
        switch (context.SpellType)
        {
            case SpellType.Projtile:
                if (spellInstance.TryGetComponent<SpellProjectile>(out var projectile))
                {
                    projectile.Initialize(context);
                }
                break;
            case SpellType.Particle:
                if (spellInstance.TryGetComponent<SpellParticle>(out var particle))
                {
                    particle.Initialize(context);
                }
                break;
            default:
                break;
        }
        

        return spellInstance;
    }
}