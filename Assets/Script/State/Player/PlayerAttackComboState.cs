using UnityEngine;

public class PlayerAttackComboState : PlayerState
{
    private int currentComboIndex= 0;
    private bool isHeavyAttackInputBuffered = false;
    private bool isComboInputBuffered = false;
    private float comboInputBufferTimeMin = 0.3f; // 콤보 입력 버퍼 최소시간
    private float stateNormalizedTimeMax = 0.85f; // 콤보 입력 버퍼 최대시간
    private readonly LightAttackStepData[] comboSteps;
    public PlayerAttackComboState(Player player, LightAttackStepData[] steps):base(player)
    {
        this.comboSteps = steps;
    }
    override public void OnEnter()
    {
        
        
        player.Status.RemoveFlag(CharacterActionFlags.CanMove);
        player.Status.RemoveFlag(CharacterActionFlags.CanRoll);
        player.Status.RemoveFlag(CharacterActionFlags.CanRotate);
        player.Status.RemoveFlag(CharacterActionFlags.CanJump);
        PlayComboAnimation();
    }
    override public void Update()
    {
        
        AnimatorStateInfo stateInfo = player.Animator.GetCurrentAnimatorStateInfo(0);
        //패리 입력 처리 일반 공격 중 패리 연계
        if (player.Input.ParryTriggered)
        {
            player.Controller.ParryState.SetNextComboIndex(currentComboIndex + 1 < comboSteps.Length ? currentComboIndex + 1 : currentComboIndex);
            player.Controller.ActionFSM.ChangeState(player.Controller.ParryState);
            return;
        }
        if (player.Input.HeavyAttackTriggered)
        {
            isHeavyAttackInputBuffered = true;
        }
        
        //일반 공격 연타 입력 버퍼링
        if (stateInfo.normalizedTime >= comboInputBufferTimeMin && player.Input.AttackTriggered)
        {
            isComboInputBuffered = true;
        }

        //애니메이션 종료 시점에 다음 콤보 or 종료처리
        if (stateInfo.normalizedTime>= stateNormalizedTimeMax)
        {
            //강공격 입력 처리 콤보 종료 이후 강공격 전환
            if (isHeavyAttackInputBuffered&&player.ResourceSystem.UseStaminaEasy(player.Controller.HeavyAttackState.StaminaCost(currentComboIndex)))
            {
                player.Controller.HeavyAttackState.SetStepIndex(currentComboIndex);
                player.Controller.ActionFSM.ChangeState(player.Controller.HeavyAttackState);
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
                player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
                return;
            }
            else
            {
                player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
                return;
            }
        }
    }
    public override void OnExit()
    {
        player.Status.AddFlag(CharacterActionFlags.CanMove);
        player.Status.AddFlag(CharacterActionFlags.CanRoll);
        player.Status.AddFlag(CharacterActionFlags.CanRotate);
        player.Status.AddFlag(CharacterActionFlags.CanJump);
        currentComboIndex = 0;
        if (player.Attack.MainWeapon != null) 
        {
            player.Attack.MainWeapon.WeaponHitBox.DisableHitBox();
        } 
    }
    private void PlayComboAnimation()
    {
        // 스태미너 소모에 실패한 경우 상태 취소 및 탈출
        if (!player.ResourceSystem.UseStaminaEasy(comboSteps[currentComboIndex].staminaCost))
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
            return;
        }
        isComboInputBuffered = false;
        isHeavyAttackInputBuffered = false;
        // 공격(콤보 스텝) 시작 직전 입력 방향으로 1회성 회전
        ApplyAttackDirectionSnap();

        LightAttackStepData stepData = comboSteps[currentComboIndex];
        string anim = stepData.animName;

        player.Animator.CrossFade(anim, 0f);
    }
    public void SetComboIndex(int index)
    {
        currentComboIndex = index;
    }
    private void ApplyAttackDirectionSnap()
    {
        if (player.Status.IsLockOn)
        {
            player.Move.RotateSnapTowardsTarget(player.lockOnTarget.position);
            return;
        }

        Vector3 rawInput = player.Input.MoveInput;
        if (rawInput.sqrMagnitude <= 0.001f) return;

        Vector3 worldDir = player.Move.GetWorldDirection(rawInput);
        player.Move.RotateSnap(worldDir);
    }
}
