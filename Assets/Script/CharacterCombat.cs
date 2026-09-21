using UnityEditor;
using UnityEngine;

public  abstract class CharacterCombat : MonoBehaviour
{
    public virtual void ResolveHit(AttackData attackData, Transform attacker, HitPart hitPart)
    {
        Debug.Log("ResolveHit");
    }
}
