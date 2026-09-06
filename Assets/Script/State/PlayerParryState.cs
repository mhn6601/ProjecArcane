using UnityEngine;

public class PlayerParryState : PlayerState
{
    private static readonly int ParryHash = Animator.StringToHash("Parry");
    private static readonly int LocomotionTreeHash = Animator.StringToHash("LocomotionTree");
    private int nextComboIndex = 0;
    private bool isParrysuccess = false;
    private float stateTimer = 0f;
    private float parryWindowDuration = 0.5f; // 패리 성공 판정 시간
    private float recoveryDelayOnFail = 0.8f;// 패리 실패 시 회복 지연 시간
    public PlayerParryState(PlayerController controller) : base(controller)
    {
    }
    public override void OnEnter()
    {
        stateTimer = 0f;
        isParrysuccess = false;
        controller.Status.RemoveFlag(CharacterActionFlags.CanMove);
        controller.Status.RemoveFlag(CharacterActionFlags.CanJump);
        controller.PlayerMove.ResetCache();
        controller.Animator.CrossFade(ParryHash, 0f);
    }
    public override void Update()
    {
        stateTimer += Time.deltaTime;
        if (isParrysuccess)
        {
            controller.AttackComboState.SetComboIndex(nextComboIndex);
            if(controller.Input.AttackTriggered)
            {
                controller.ActionFSM.ChangeState(controller.AttackComboState);
                return;
            }
        }
        if(stateTimer>parryWindowDuration)
        {
            //패리 실패 시 회복 지연 시간 후 대기 상태 복귀
            if (stateTimer > parryWindowDuration + recoveryDelayOnFail)
            {
                controller.ActionFSM.ChangeState(controller.EmptyActionState);

                controller.Animator.CrossFade(LocomotionTreeHash, 0f);

            }
        }
    }

    public override void OnExit()
    {
        controller.Status.AddFlag(CharacterActionFlags.CanMove);
        controller.Status.AddFlag(CharacterActionFlags.CanJump);
    }
    public void OnParrySuccess()
    {
        isParrysuccess = true;
    }
    public void SetNextComboIndex(int index)=>nextComboIndex = index;
}
