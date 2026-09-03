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

    public PlayerLocomotionState(PlayerController controller): base(controller) 
    {
    }

    public override void OnEnter()
    {
       // Debug.Log("PlayerLocomotionState OnEnter");
        controller.Animator.CrossFade(LocomotionHash, 0.1f);
        controller.Animator.SetBool(GroundedBoolHash, true);
    }

    public override void Update()
    {
        if (!controller.Status.CanMove)
        {
            return;
        }
        if (controller.Input.JumpTriggered)
        {
            controller.MovementFSM.ChangeState(controller.AirborneState);
            return;
        }
        if (!controller.PlayerJump.IsGround())
        {
            controller.MovementFSM.ChangeState(controller.AirborneState);
            return;
        }
        Vector3 moveInput = controller.Input.MoveInput;
        bool isRunPressed = controller.Input.IsRunPressed;

        // 1. 단순 0 체크 optimization (.sqrMagnitude 활용)
        if (moveInput.sqrMagnitude <= 0.001f)
        {
            // 입력이 없으면 애니메이션 파라미터를 0으로 보간 후 이동 중단
            SetAnimatorParametersMoveHash(0, 0.1f);
            controller.PlayerMove.ExecuteMove(Vector2.zero, isRunPressed);
            UpdateAnimatorParameters(moveInput, 0f);
            return;
        }

        
        // 2. 장치별 속도 배율(Multiplier) 계산 (걷기: 0.5 / 뛰기: 1.0)
        float inputMagnitude = moveInput.magnitude;
        float targetSpeedMultiplier = 0f;

        if (controller.CurrentDevice == PlayerController.ControlDeviceType.Gamepad)
        {
            float maxCap = isRunPressed ? 1.0f : 0.5f;
            targetSpeedMultiplier = Mathf.Clamp(inputMagnitude, 0f, maxCap);
        }
        else
        {
            targetSpeedMultiplier = isRunPressed ? 1.0f : 0.5f;
        }
        UpdateAnimatorParameters(moveInput, targetSpeedMultiplier);
        if (controller.Status.IsLockOn)
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
        controller.PlayerMove.ExecuteMove(moveInput, isRunPressed);
        // 5. 2D Blend Tree 파라미터 업데이트 (MoveX, MoveZ)
    }

    public override void OnExit()
    {
       // Debug.Log("PlayerLocomotionState OnExit");
        // 상태 탈출 시 애니메이션 축 초기화

        controller.PlayerMove.ResetCache();
    }
    private void UpdateAnimatorParameters(Vector3 moveInput, float speedMultiplier)
    {
        // 걷기/뛰기 상태에 따라 Bool 파라미터 설정
        controller.Animator.SetBool(IdleBoolHash, speedMultiplier == 0f);
        controller.Animator.SetBool(WalkBoolHash, speedMultiplier > 0f && speedMultiplier < 1f);
        controller.Animator.SetBool(RunBoolHash, speedMultiplier >= 1f);
    }
    private void SetAnimatorParametersMoveHash(Vector2 animInput, float dampTime)
    {
        controller.Animator.SetFloat(MoveXHash, animInput.x, dampTime, Time.fixedDeltaTime);
        controller.Animator.SetFloat(MoveZHash, animInput.y, dampTime, Time.fixedDeltaTime); 
    }
     private void SetAnimatorParametersMoveHash(float value, float dampTime)
    {
        controller.Animator.SetFloat(MoveXHash, value, dampTime, Time.fixedDeltaTime);
        controller.Animator.SetFloat(MoveZHash, value, dampTime, Time.fixedDeltaTime);
    }

}