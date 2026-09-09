using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public class LightAttackStepData
{
    public string animName;
    [Tooltip("무기 기본 데미지에 곱해지는 배율 (콤보 후반 타수일수록 강화 가능)")]
    public float damageMultiplier = 1f;
    public float staminaCost = 10f;
    
    public float parryWindowStart=0.1f;
    public float parryWindowEnd=0.3f;
}
[System.Serializable]
public class HeavyAttackStepData 
{
    public string animName;
    public float damageMultiplier = 1.5f;
    public float staminaCost = 15f;
}
