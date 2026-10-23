using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Death / Respawn")]
    [SerializeField] private Transform respawnPoint;
    private Vector3 respawnPosition;

    public event Action PlayerDied;
    public event Action PlayerRespawned;

    private void Start()
    {
        respawnPosition = respawnPoint != null
            ? respawnPoint.position
            : Player.Instance.transform.position;
    }

    public void SetRespawnPoint(Vector3 position) => respawnPosition = position;

    public void OnPlayerDied()
    {
        PlayerDied?.Invoke();
    }

    public void RespawnPlayer()
    {

        Player.Instance.Respawn(respawnPosition);
        PlayerRespawned?.Invoke();
    }
}