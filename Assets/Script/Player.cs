using UnityEngine;
public class Player:Singleton<Player>
{
    public bool isLockOn=false;
    public bool isWalking = false;
    protected override void Awake()
    {
        base.Awake();
    }
}
