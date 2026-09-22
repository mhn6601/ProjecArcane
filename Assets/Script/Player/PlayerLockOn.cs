using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerLockOn : MonoBehaviour
{
    private Player player;
    public Transform CurrentTarget;
    private void Awake()
    {
        player = Player.Instance;
    }
    public void HandleLookInput(Vector2 LookInput)
    {

    }
}