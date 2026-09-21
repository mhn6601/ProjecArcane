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
    [SerializeField] public CharacterCombat owner; // 이 파츠가 속한 캐릭터

    [SerializeField]
    private HitPart hitPart;

    public HitPart HitPart => hitPart;
    
    public void SetHitPart(HitPart hitPart)
    {
        this.hitPart = hitPart;
    }
}