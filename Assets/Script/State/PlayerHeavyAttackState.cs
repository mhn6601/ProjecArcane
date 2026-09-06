using UnityEngine;

public class PlayerHeavyAttackState : PlayerState
{
    private float stateNormalizedTimeMax = 0.85f; // 콤보 입력 버퍼 최대시간
    private readonly HeavyAttackStepData[] heavySteps;
    private int stepIndex = 0;

    public PlayerHeavyAttackState(PlayerController controller, HeavyAttackStepData[] steps) : base(controller)
    {
        this.heavySteps = steps;
    }

    public void SetStepIndex(int index)
    {
        stepIndex = Mathf.Clamp(index, 0, heavySteps.Length - 1);
    }

    public override void OnEnter()
    {
        controller.Status.RemoveFlag(CharacterActionFlags.CanMove);
        controller.Status.RemoveFlag(CharacterActionFlags.CanJump);
        controller.Status.RemoveFlag(CharacterActionFlags.CanGuard);
        controller.Status.RemoveFlag(CharacterActionFlags.CanParry);

        // 현재 콤보 단계에 일치하는 강공격 데이터 가져오기
        HeavyAttackStepData currentData = heavySteps[stepIndex];

        // 애니메이션 재생 및 스태미너 소모 등의 로직 실행
        controller.Animator.CrossFade(currentData.animName, 0f);
        // controller.Status.UseStamina(currentData.staminaCost);
    }

    public override void Update()
    {
        AnimatorStateInfo stateInfo = controller.Animator.GetCurrentAnimatorStateInfo(0);

        // 애니메이션 종료 시 대기(EmptyActionState)로 복귀
        if (stateInfo.normalizedTime >= stateNormalizedTimeMax)
        {
            controller.ActionFSM.ChangeState(controller.EmptyActionState);
        }
    }
    public override void OnExit()
    {
        controller.Status.AddFlag(CharacterActionFlags.CanMove);
        controller.Status.AddFlag(CharacterActionFlags.CanJump);
        controller.Status.AddFlag(CharacterActionFlags.CanGuard);
        controller.Status.AddFlag(CharacterActionFlags.CanParry);
    }
}

