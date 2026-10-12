using UnityEditor;
using UnityEngine;

public  abstract class CharacterCombat : MonoBehaviour
{
    public virtual void ResolveHit(AttackData attackData, Transform attacker,Transform weapon, HitPart hitPart)
    {
        Debug.Log("ResolveHit");
    }
}
