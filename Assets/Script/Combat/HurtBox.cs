using UnityEditor;
using UnityEngine;
public enum HitPart
{
    Head,
    Body,
    Arm,
    Leg
}
public class HurtBox : MonoBehaviour
{
    [SerializeField] private CharacterCombat owner; // 이 파츠가 속한 캐릭터

    [SerializeField]
    private HitPart hitPart;

    public HitPart HitPart => hitPart;
    public void TakeHit(AttackData attackData, Transform attacker)
    {
        Debug.Log("hurtbox takehit");
        // 방어자 상태 우선 체크: 패링/가드 중인지 owner에게 위임
        owner.ResolveHit(attackData, attacker, HitPart);
    }
    public void SetHitPart(HitPart hitPart)
    {
        this.hitPart = hitPart;
    }
    public void SetOwner(CharacterCombat owner)
    {
        this.owner = owner;
    }
}