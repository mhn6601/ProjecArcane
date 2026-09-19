using UnityEditor;
using UnityEngine;

public  abstract class CharacterCombat : MonoBehaviour
{
    public virtual void ResolveHit(AttackData attackData, Transform attacker, BodyPart bodyPart)
    {
        Debug.Log("ResolveHit");
    }
}
