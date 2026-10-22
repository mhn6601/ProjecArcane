using UnityEngine;

[CreateAssetMenu(fileName = "RicochetSpecialNode", menuName = "Special Node/Ricochet")]
public class RicochetSpecialNode : SpecialNodeData
{
    [Header("Ricochet")]
    public int bounceCount = 2;
    public float range = 8f;
    [Range(0f, 1f)] public float damageMultiplierPerBounce = 0.7f;

    public override void ApplyNodeEffect(SpellCastContext ctx)
    {
        ctx.RecocehtCount += bounceCount;
        ctx.RecocehtRange = Mathf.Max(ctx.RecocehtRange, range);
        ctx.RecocehtDamageMultiplier *= damageMultiplierPerBounce;
    }
}