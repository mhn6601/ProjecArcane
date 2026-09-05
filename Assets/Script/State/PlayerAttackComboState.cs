using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackComboState : PlayerState
{
    private int currentComboIndex= 0;
    private bool isHeavyAttackInputBuffered = false;
    private bool isComboInputBuffered = false;
    private float comboInputBufferTimeMin = 0.3f; // 콤보 입력 버퍼 최소시간
    private float stateNormalizedTimeMax = 0.85f; // 콤보 입력 버퍼 최대시간
    private readonly LightAttackStepData[] comboSteps;
    public PlayerAttackComboState(PlayerController controller, LightAttackStepData[] steps):base(controller)
    {
        this.comboSteps = steps;
    }
    override public void OnEnter()
    {

        controller.Status.RemoveFlag(CharacterActionFlags.CanMove);
        controller.Status.RemoveFlag(CharacterActionFlags.CanJump);
        PlayComboAnimation();
    }
    override public void Update()
    {
        
        AnimatorStateInfo stateInfo = controller.Animator.GetCurrentAnimatorStateInfo(0);
        //패리 입력 처리 일반 공격 중 패리 연계
        if (controller.Input.ParryTriggered)
        {
            controller.ParryState.SetNextComboIndex(currentComboIndex + 1 < comboSteps.Length ? currentComboIndex + 1 : currentComboIndex);
            controller.ActionFSM.ChangeState(controller.ParryState);
            return;
        }
        if (controller.Input.HeavyAttackTriggered)
        {
            isHeavyAttackInputBuffered = true;
        }
        
        //일반 공격 연타 입력 버퍼링
        if (stateInfo.normalizedTime >= comboInputBufferTimeMin && controller.Input.AttackTriggered)
        {
            isComboInputBuffered = true;
        }

        //애니메이션 종료 시점에 다음 콤보 or 종료처리
        if (stateInfo.normalizedTime>= stateNormalizedTimeMax)
        {
            //강공격 입력 처리 콤보 종료 이후 강공격 전환
            if (isHeavyAttackInputBuffered)
            {
                controller.HeavyAttackState.SetStepIndex(currentComboIndex);
                controller.ActionFSM.ChangeState(controller.HeavyAttackState);
                return;
            }
            if (isComboInputBuffered&&currentComboIndex<comboSteps.Length-1)
            {
                currentComboIndex++;
                
                PlayComboAnimation();
            }
            else if(isComboInputBuffered)
            {   
                //콤보 종료 이후 대기 상태 복귀
                controller.ActionFSM.ChangeState(controller.EmptyActionState);
                return;
            }
            else
            {
                controller.ActionFSM.ChangeState(controller.EmptyActionState);
                return;
            }
        }
    }
    public override void OnExit()
    {
        controller.Status.AddFlag(CharacterActionFlags.CanMove);
        controller.Status.AddFlag(CharacterActionFlags.CanJump);
        currentComboIndex = 0;
    }
    private void PlayComboAnimation()
    {
        isComboInputBuffered= false;
        isHeavyAttackInputBuffered = false;
        string anim = comboSteps[currentComboIndex].animName;
        controller.Animator.CrossFade(anim, 0f);
    }
    public void SetComboIndex(int index)
    {
        currentComboIndex = index;
    }
}
