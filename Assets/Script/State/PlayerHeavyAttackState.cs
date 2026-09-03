using UnityEngine;

public class PlayerHeavyAttackState : PlayerState
{
    private readonly HeavyAttackStepData[] comboSteps;
    public PlayerHeavyAttackState(PlayerController controller, HeavyAttackStepData[] steps) : base(controller)
    {
        this.comboSteps = steps;
    }   
    

    public override void OnEnter()
    {
    }
    public override void Update()
    {
    }

    public override void OnExit()
    {
    }
    public void SetStepIndex(int index)
    {
    }
}

