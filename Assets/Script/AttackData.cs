using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class AttackData
{
    public string animName;

    [Tooltip("스텝 데미지 배율 (최종 데미지 = (무기 물리 데미지 + 공격력) × 배율)")]
    [FormerlySerializedAs("damage")]
    public float damageMultiplier = 1f;

    /// <summary>타격 시점에 계산되어 적에게 전달되는 최종 데미지. 저장되지 않는 런타임 값.</summary>
    [NonSerialized] public float damage;

    public ElementType element;
    public float staminaCost = 10f;
    public float knockbackForce = 0f;
    public float hitboxSize = 1f;
    public bool canBeParried = true;
    public int activeFrameStart = 0;
    public int activeFrameEnd = 0;

    /// <summary>에셋 데이터를 건드리지 않고, 최종 데미지가 채워진 복사본을 만든다.</summary>
    public AttackData CreateHit(float finalDamage)
    {
        AttackData hit = (AttackData)MemberwiseClone();
        hit.damage = finalDamage;
        return hit;
    }
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
