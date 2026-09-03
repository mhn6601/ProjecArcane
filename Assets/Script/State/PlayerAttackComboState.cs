using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackComboState : PlayerState
{
    private int currentComboIndex= 0;
    private bool isComboInputBuffered = false;
    private readonly LightAttackStepData[] comboSteps;
    public PlayerAttackComboState(PlayerController controller, LightAttackStepData[] steps):base(controller)
    {
        this.comboSteps = steps;
    }
    override public void OnEnter()
    {
        PlayComboAnimation();
    }
    override public void Update()
    {
        AnimatorStateInfo stateInfo = controller.Animator.GetCurrentAnimatorStateInfo(1);
        //강공격 입력 처리 콤보 종료 이후 강공격 전환
        if (controller.Input.HeavyAttackTriggered)
        {
            controller.HeavyAttackState.SetStepIndex(currentComboIndex);
            controller.ActionFSM.ChangeState(controller.HeavyAttackState);
            return;
        }

        //패리 입력 처리 일반 공격 중 패리 연계
        if (controller.Input.ParryTriggered)
        {
            controller.ParryState.SetNextComboIndex(currentComboIndex + 1);
            controller.ActionFSM.ChangeState(controller.ParryState);
            return;
        }

        //일반 공격 연타 입력 버퍼링
        if (controller.Input.AttackTriggered)
        {
            isComboInputBuffered = true;
        }

        //애니메이션 종료 시점에 다음 콤보 or 종료처리
        if (stateInfo.normalizedTime>=0.85f)
        {
            if(isComboInputBuffered&&currentComboIndex<comboSteps.Length-1)
            {
                currentComboIndex++;
                PlayComboAnimation();
            }
            else
            {   
                //콤보 종료 이후 대기 상태 복귀
                controller.ActionFSM.ChangeState(controller.EmptyActionState);
            }
        }
        
    }
    public override void OnExit()
    {
    }
    private void PlayComboAnimation()
    {
        isComboInputBuffered= false;
        string anim = comboSteps[currentComboIndex].animName;
        controller.Animator.CrossFade(anim, 0.1f);
        controller.Status.RemoveFlag(CharacterActionFlags.CanMove);
    }
}
