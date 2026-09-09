using UnityEngine;

public class PlayerAttackData : AttackData
{
    [Header("Light Attack Combo (1 ~ 4 Step)")]
    public LightAttackStepData[] lightAttackSteps = new LightAttackStepData[4];

    [Header("Heavy Attack Combo Extensions")]
    public HeavyAttackStepData[] heavyAttackSteps = new HeavyAttackStepData[4];
}
