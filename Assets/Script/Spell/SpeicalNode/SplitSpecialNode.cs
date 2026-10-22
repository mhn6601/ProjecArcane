using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SplitNode", menuName = "Special Node/Split")]
public class SplitSpecialNode : SpecialNodeData
{
    [Min(2)] public int count = 3;
    public float angleStep = 15f;
    [Range(0.1f, 1f)] public float damageMultiplierPerShot = 0.6f;

    public override void ApplyNodeEffect(SpellCastContext ctx)
    {
        ctx.SplitCount = Mathf.Max(ctx.SplitCount, count);
        ctx.SplitAngleStep = angleStep;
        ctx.SplitDamageMultiplier *= damageMultiplierPerShot;
    }
}