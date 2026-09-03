using UnityEngine;
public class Player:Singleton<Player>
{
    #region playerStat

    [SerializeField] private float moveSpeed= 2.5f;
    public float MoveSpeed {get {return moveSpeed;}}

    [SerializeField] private float runSpeed =5f;
    public float RunSpeed {get {return runSpeed;}}
    #endregion
    public Transform lockOnTarget;
}
