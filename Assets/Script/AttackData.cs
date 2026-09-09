using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Combat/Attack Data")]
public class AttackData:ScriptableObject
{
    public string animName;
    public float damage=0f;
    public float damageMultiplier= 1.0f;
    public float guardDamageMultiplier= 1.0f;
    public float knockbackForce=0f;
    
    public float hitboxSize=1f;
    public int activeFrameStart = 0;
    public int activeFrameEnd = 0;
    public bool canBeParried = false;    
}
public enum AttackType
{
    Light,
    Heavy,
    Magic,
    Special
}
