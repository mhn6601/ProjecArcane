using UnityEngine;
using System;

[Serializable]
public class AttackData
{
    public string animName;
    public float damage = 1f;
    public ElementType element ; 
    public float staminaCost = 10f;
    public float knockbackForce = 0f;
    public float hitboxSize = 1f;
    public bool canBeParried = true;
    public int activeFrameStart = 0;
    public int activeFrameEnd = 0;
    
}
[Serializable]
public class LightAttackStepData : AttackData
{
    public AttackType attackType = AttackType.Light;
    public float parryWindowStart = 0.1f;
    public float parryWindowEnd = 0.3f;

}
[Serializable]
public class HeavyAttackStepData : AttackData
{
    public AttackType attackType = AttackType.Heavy;
}

public enum AttackType
{
    Light,
    Heavy,
    Magic,
    Special
}
