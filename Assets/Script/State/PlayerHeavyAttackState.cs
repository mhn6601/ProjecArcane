using UnityEngine;

public class PlayerHeavyAttackState : PlayerState
{
    private float stateNormalizedTimeMax = 0.85f; // 콤보 입력 버퍼 최대시간
    private readonly HeavyAttackStepData[] heavySteps;
    private int stepIndex = 0;

    public PlayerHeavyAttackState(Player player, HeavyAttackStepData[] steps) : base(player)
    {
        this.heavySteps = steps;
    }

    public void SetStepIndex(int index)
    {
        stepIndex = Mathf.Clamp(index, 0, heavySteps.Length - 1);
    }

    public override void OnEnter()
    {
        player.Status.RemoveFlag(CharacterActionFlags.CanMove);
        player.Status.RemoveFlag(CharacterActionFlags.CanJump);
        player.Status.RemoveFlag(CharacterActionFlags.CanGuard);
        player.Status.RemoveFlag(CharacterActionFlags.CanParry);

        // 현재 콤보 단계에 일치하는 강공격 데이터 가져오기
        HeavyAttackStepData currentData = heavySteps[stepIndex];

        // 애니메이션 재생 및 스태미너 소모 등의 로직 실행
        player.Animator.CrossFade(currentData.animName, 0f);
        // player.Status.UseStamina(currentData.staminaCost);
    }

    public override void Update()
    {
        AnimatorStateInfo stateInfo = player.Animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 종료 시 대기(EmptyActionState)로 복귀
        if (stateInfo.normalizedTime >= stateNormalizedTimeMax)
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
        }
    }
    public override void OnExit()
    {
        player.Status.AddFlag(CharacterActionFlags.CanMove);
        player.Status.AddFlag(CharacterActionFlags.CanJump);
        player.Status.AddFlag(CharacterActionFlags.CanGuard);
        player.Status.AddFlag(CharacterActionFlags.CanParry);
    }
    public float StaminaCost(int comboIndex)
    {
        return heavySteps[comboIndex].staminaCost;
    }
}

