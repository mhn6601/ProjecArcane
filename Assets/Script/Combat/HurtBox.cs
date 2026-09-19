using UnityEditor;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [SerializeField] private CharacterCombat owner; // 이 파츠가 속한 캐릭터
    [SerializeField] private BodyPart bodyPart; // Head, Torso, Limb 등 (부위별 배율용)

    public void TakeHit(AttackData attackData, Transform attacker)
    {
        Debug.Log("hurtbox takehit");
        // 방어자 상태 우선 체크: 패링/가드 중인지 owner에게 위임
        owner.ResolveHit(attackData, attacker, bodyPart);
    }
}