using UnityEngine;
public class Player:Singleton<Player>
{
    #region playerStatus
    [Header("Player Status")]
    public bool isGrounded = false;
    public bool isLockOn=false;

    [SerializeField] private float moveSpeed= 2.5f;
    public float MoveSpeed {get {return moveSpeed;}}

    [SerializeField] private float runSpeed =5f;
    public float RunSpeed {get {return runSpeed;}}
    #endregion
    public Transform lockOnTarget;
}
