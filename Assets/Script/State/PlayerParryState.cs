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
    public PlayerParryState(Player player) : base(player)
    {
    }
    public override void OnEnter()
    {
        stateTimer = 0f;
        isParrysuccess = false;
        player.Status.RemoveFlag(CharacterActionFlags.CanMove);
        player.Status.RemoveFlag(CharacterActionFlags.CanJump);
        player.Controller.Animator.CrossFade(ParryHash, 0f);
    }
    public override void Update()
    {
        stateTimer += Time.deltaTime;
        if (isParrysuccess)
        {
            player.Controller.AttackComboState.SetComboIndex(nextComboIndex);
            if(player.Input.AttackTriggered)
            {
                player.Controller.ActionFSM.ChangeState(player.Controller.AttackComboState);
                return;
            }
        }
        if(stateTimer>parryWindowDuration)
        {
            //패리 실패 시 회복 지연 시간 후 대기 상태 복귀
            if (stateTimer > parryWindowDuration + recoveryDelayOnFail)
            {
                player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);

                player.Controller.Animator.CrossFade(LocomotionTreeHash, 0f);

            }
        }
    }

    public override void OnExit()
    {
        player.Status.AddFlag(CharacterActionFlags.CanMove);
        player.Status.AddFlag(CharacterActionFlags.CanJump);
    }
    public void OnParrySuccess()
    {
        isParrysuccess = true;
    }
    public void SetNextComboIndex(int index)=>nextComboIndex = index;
}
