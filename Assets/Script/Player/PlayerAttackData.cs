using UnityEngine;

[CreateAssetMenu(fileName ="PlayerAttackData",menuName ="Combat/Player Attack Data")]
public class PlayerAttackData : ScriptableObject
{
    [Header("Light Attack Combo (1 ~ 4 Step)")]
    public LightAttackStepData[] lightAttackSteps = new LightAttackStepData[4];

    [Header("Heavy Attack Combo Extensions")]
    public HeavyAttackStepData[] heavyAttackSteps = new HeavyAttackStepData[4];
}
