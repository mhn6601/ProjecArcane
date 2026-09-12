using UnityEngine;

public class PlayerLocomotionState : PlayerState
{
    private static readonly int LocomotionHash = Animator.StringToHash("LocomotionTree");

    private static readonly int GroundedBoolHash = Animator.StringToHash("isGrounded");

    // 2D Blend Tree를 위한 X, Y 파라미터 해시 선언
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveZHash = Animator.StringToHash("MoveZ");
    private static readonly int IdleBoolHash = Animator.StringToHash("isIdle");
    private static readonly int WalkBoolHash = Animator.StringToHash("isWalk");
    private static readonly int RunBoolHash = Animator.StringToHash("isRun");

    public PlayerLocomotionState(Player player): base(player) 
    {
    }

    public override void OnEnter()
    {
        // Debug.Log("PlayerLocomotionState OnEnter");
        player.Animator.CrossFade(LocomotionHash, 0.1f);
        player.Animator.SetBool(GroundedBoolHash, true);
    }

    public override void Update()
    {
        Vector3 moveInput = player.Input.MoveInput;
        if (!player.Status.CanMove)
        {
            return;
        }
        if (player.Input.JumpTriggered)
        {
            player.Controller.MovementFSM.ChangeState(player.Controller.JumpState);
            return;
        }
        if (!player.Jump.IsGround())
        {
            player.Controller.MovementFSM.ChangeState(player.Controller.AirborneState);
            return;
        }
        
        bool isRunPressed = player.Input.IsRunPressed;
        bool isMoving = moveInput.sqrMagnitude > 0.001f;
        bool canRun = false;
        if (isMoving && isRunPressed)
        {
            // 스태미너 소모 시도 (차감 성공 시 true 반환)
            canRun = player.ResourceSystem.ConsumeStaminaContinous(player.Stats.RunStaminaCostPerSec);
        }
        // 스태미너가 없거나 입력이 없으면 isRunPressed 판정을 false로 보정하여 걷기로 전환
        bool actualRunState = isRunPressed && canRun;

        if (actualRunState)
        {
            player.Status.RemoveFlag(CharacterActionFlags.CanGuard);
        }
        else
        {
            player.Status.AddFlag(CharacterActionFlags.CanGuard);

        }
        float gravity= player.Jump.GroundedGravity;
        Vector3 gravityVector = new Vector3(0, gravity, 0);
        // 1. 단순 0 체크 optimization (.sqrMagnitude 활용)
        if (!isMoving)
        {
            // 입력이 없으면 애니메이션 파라미터를 0으로 보간 후 이동 중단
            SetAnimatorParametersMoveHash(0, 0.1f);
            
            player.Move.ExecuteMove(gravityVector, false);
            UpdateAnimatorParameters(moveInput, 0f);
            return;
        }

        
        // 2. 장치별 속도 배율(Multiplier) 계산 (걷기: 0.5 / 뛰기: 1.0)
        float inputMagnitude = moveInput.magnitude;
        float targetSpeedMultiplier = 0f;

        if (player.Controller.CurrentDevice == PlayerController.ControlDeviceType.Gamepad)
        {
            float maxCap = isRunPressed ? 1.0f : 0.5f;
            targetSpeedMultiplier = Mathf.Clamp(inputMagnitude, 0f, maxCap);
        }
        else
        {
            targetSpeedMultiplier = actualRunState ? 1.0f : 0.5f;
        }
        UpdateAnimatorParameters(moveInput, targetSpeedMultiplier);
        if (player.Status.IsLockOn)
        {
            var animInput=Vector2.zero;
            SetAnimatorParametersMoveHash(animInput,0.1f);
        }
        else
        {
            //var animInput= moveInput.sqrMagnitude > 0.001f ? targetSpeedMultiplier : 0f;
            //Debug.Log(animInput);
            SetAnimatorParametersMoveHash(targetSpeedMultiplier, 0.1f);
        }
        // 4. 이동 로직 위임
        player.Move.ExecuteMove(moveInput+ gravityVector, actualRunState);
        // 5. 2D Blend Tree 파라미터 업데이트 (MoveX, MoveZ)
    }

    public override void OnExit()
    {
       // Debug.Log("PlayerLocomotionState OnExit");
        // 상태 탈출 시 애니메이션 축 초기화

    }
    private void UpdateAnimatorParameters(Vector3 moveInput, float speedMultiplier)
    {
        // 걷기/뛰기 상태에 따라 Bool 파라미터 설정
        player.Animator.SetBool(IdleBoolHash, speedMultiplier == 0f);
        player.Animator.SetBool(WalkBoolHash, speedMultiplier > 0f && speedMultiplier < 1f);
        player.Animator.SetBool(RunBoolHash, speedMultiplier >= 1f);
    }
    private void SetAnimatorParametersMoveHash(Vector2 animInput, float dampTime)
    {
        player.Animator.SetFloat(MoveXHash, animInput.x, dampTime, Time.fixedDeltaTime);
        player.Animator.SetFloat(MoveZHash, animInput.y, dampTime, Time.fixedDeltaTime); 
    }
     private void SetAnimatorParametersMoveHash(float value, float dampTime)
    {
        player.Animator.SetFloat(MoveXHash, value, dampTime, Time.fixedDeltaTime);
        player.Animator.SetFloat(MoveZHash, value, dampTime, Time.fixedDeltaTime);
    }

}