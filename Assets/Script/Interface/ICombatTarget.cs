using UnityEditor;
using UnityEngine;

public interface ICombatTarget
{
    void ApplyStagger(float amount);
    void ResolveHit(AttackData attackData, Transform attacker, BodyPart bodyPart);
    bool IsDead { get; }
}
