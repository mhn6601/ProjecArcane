using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    private static readonly int LandingHash = Animator.StringToHash("Falling To Landing");
    // 공중 유지 동안 사용할 관성 캐시 데이터
    // 랜딩 애니메이션이 끝났는지 못 잡는 경우를 대비한 안전 타임아웃
    private const float MaxLandingTime = 1.0f;
    // 랜딩 애니메이션이 이 비율 이상 재생되면 종료로 간주
    private const float LandingEndNormalizedTime = 0.9f;

    // 공중 유지 동안 사용할 관성 캐시 데이터
    private Vector3 initialJumpDirection;
    private bool initialIsRun;

    // 착지 단계 관련
    private bool isLanding;
    private float landingTimer;

    // 이 시간(초) 이상 떨어졌을 때만 착지 애니메이션 재생
    private const float MinFallTimeForLanding = 0.5f;

    // 하강 구간(수직 속도 < 0) 누적 시간
    private float fallTimer;
    public PlayerAirborneState(Player player, Animator animator) : base(player, animator)
    {
    }

    // JumpState 등 외부에서 관성 데이터를 주입받는 메서드
    public void SetInertiaData(Vector3 direction, bool isRun)
    {
        initialJumpDirection = direction;
        initialIsRun = isRun;
    }

    public override void OnEnter()
    {
        isLanding = false;
        landingTimer = 0f;
        fallTimer = 0f;
        if (player.Controller.ActionFSM.CurrentState == player.Controller.GuardState)
        {
            player.Controller.ActionFSM.ChangeState(player.Controller.EmptyActionState);
        }

        player.Status.RemoveFlag(CharacterActionFlags.CanMove);
        player.Status.RemoveFlag(CharacterActionFlags.CanRotate);
        player.Status.RemoveFlag(CharacterActionFlags.CanRoll);
        player.Status.RemoveFlag(CharacterActionFlags.CanJump);
        player.Status.RemoveFlag(CharacterActionFlags.CanGuard);
        player.Status.RemoveFlag(CharacterActionFlags.CanParry);
        player.Status.RemoveFlag(CharacterActionFlags.CanAttack);
        player.Status.RemoveFlag(CharacterActionFlags.CanCastSpell);
    }

    public override void Update()
    {
        // 착지 단계: 공중 이동은 멈추고 전환 조건만 확인
        if (isLanding)
        {
            UpdateLanding();
            return;
        }

        // 1. 공중 이동 처리 (캐싱된 관성으로 유지 이동)
        player.Jump.ExecuteJump(initialJumpDirection, initialIsRun);

        // 하강 중일 때만 낙하 시간 누적 (점프 상승 구간은 제외)
        if (player.Jump.GetJumpVelocity() < 0f)
        {
            fallTimer += Time.deltaTime;
        }

        // 2. 착지 판정
        if (player.Jump.GetJumpVelocity() <= 0f && player.Jump.IsGround())
        {
            if (fallTimer >= MinFallTimeForLanding)
            {
                Debug.Log("doing landing");
                // 오래 떨어졌으면 랜딩 애니메이션 재생
                EnterLanding();
            }
            else
            {
                // 짧게 떨어졌으면 랜딩 없이 바로 Locomotion
                ExitToLocomotion();
            }
        }
    }
    private void EnterLanding()
    {
        isLanding = true;
        landingTimer = 0f;

        // 공중 관성 제거 (착지 중에는 더 이상 밀리지 않도록)
        initialJumpDirection = Vector3.zero;
        initialIsRun = false;

        animator.CrossFade(LandingHash, 0.1f);
    }

    private void UpdateLanding()
    {
        landingTimer += Time.deltaTime;
        // 랜딩 도중 지면이 사라지면 다시 공중 단계로 복귀 (관성은 이미 0이라 수직 낙하)
        if (!player.Jump.IsGround())
        {
            isLanding = false;
            return;
        }
        // 랜딩 도중에도 지면에 붙어 있도록 접지 중력 적용
        ApplyGroundedGravity();
        // 입력이 있으면 즉시 Locomotion으로
        if (HasMoveInput())
        {
            ExitToLocomotion();
            return;
        }

        // 입력이 없으면 랜딩 애니메이션이 끝난 뒤 Locomotion으로
        if (IsLandingAnimationFinished() || landingTimer >= MaxLandingTime)
        {
            ExitToLocomotion();
        }
    }
    private void ApplyGroundedGravity()
    {
        // TODO: 프로젝트의 실제 CharacterController / GroundedGravity 참조로 교체하세요.
        // GroundedGravity(-8)는 속도(m/s) 값이므로 deltaTime을 곱해 Move에 넘깁니다.
        Vector3 gravityMove = Vector3.up * (player.Jump.GroundedGravity * Time.deltaTime);
        player.Controller.CC.Move(gravityMove);
    }
    private bool IsLandingAnimationFinished()
    {
        // 랜딩으로 크로스페이드 중이면 아직 끝난 게 아님
        if (animator.IsInTransition(0))
        {
            return animator.GetNextAnimatorStateInfo(0).shortNameHash != LandingHash
                   && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == LandingHash;
        }

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        return info.shortNameHash == LandingHash && info.normalizedTime >= LandingEndNormalizedTime;
    }
    private bool HasMoveInput()
    {
        // TODO: 프로젝트의 실제 이동 입력 참조로 교체하세요.
        // 예) return player.Input.MoveInput.sqrMagnitude > 0.01f;
        return player.Input.MoveInput.sqrMagnitude > 0.01f;
    }

    private void ExitToLocomotion()
    {
        player.Controller.MovementFSM.ChangeState(player.Controller.locomotionState);
    }
    public override void OnExit()
    {
        animator.CrossFade(LandingHash, 0.1f);
        // 착지 시 상태 및 플래그 복구
        player.Status.AddFlag(CharacterActionFlags.CanMove);
        player.Status.AddFlag(CharacterActionFlags.CanRotate);
        player.Status.AddFlag(CharacterActionFlags.CanRoll);
        player.Status.AddFlag(CharacterActionFlags.CanJump);
        player.Status.AddFlag(CharacterActionFlags.CanGuard);   
        player.Status.AddFlag(CharacterActionFlags.CanParry);   
        player.Status.AddFlag(CharacterActionFlags.CanAttack);   
        player.Status.AddFlag(CharacterActionFlags.CanCastSpell);   


        // 관성 데이터 초기화
        initialJumpDirection = Vector3.zero;
        initialIsRun = false;
        isLanding = false;
        landingTimer = 0f;
        fallTimer = 0f;
    }
}