
using UnityEngine;

public  abstract class CharacterCombat : MonoBehaviour
{
    public virtual void ResolveHit(AttackData attackData, Transform attacker,Transform weapon, HitPart hitPart)
    {
        Debug.Log("ResolveHit");
    }
    /// <summary>이 캐릭터가 공격했을 때의 최종 데미지. 공격자 쪽 Combat이 구현한다.</summary>
    public virtual float CalculateDamage(AttackData step)
    {
        return step.damageMultiplier;
    }
}
