using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class LightAttackStepData
{
    public string animName;
    public float damageMultiplier = 1.0f;
    public float staminaCost = 10f;
    public float parryWindowStart=0.1f;
    public float parryWindowEnd=0.3f;
}
[System.Serializable]
public class HeavyAttackStepData 
{
    public string animName;
    public float damageMultiplier = 2.0f;
    public float staminaCost = 15f;
}
