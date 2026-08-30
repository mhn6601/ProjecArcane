using UnityEngine;

public class PlayerAirborneState : PlayerState
{
    private static readonly int AirborneSubStateHash = Animator.StringToHash("AirborneState");
    private static readonly int JumpSpeedHash = Animator.StringToHash("JumpSpeed");
    private static readonly int JumpTriggerHash = Animator.StringToHash("isJump");  
    private static readonly int GroundedBoolHash = Animator.StringToHash("isGrounded");  
    private static readonly int IdleJumpBoolHash = Animator.StringToHash("isIdleJump");  
    private static readonly int WalkJumpBoolHash = Animator.StringToHash("isWalkJump");  
    private static readonly int RunJumpBoolHash = Animator.StringToHash("isRunJump");  
    // 공중 상태 동안 유지할 관성 캐시 데이터
    private Vector3 initialJumpDirection;
    private bool initialIsRun;
    private float initialSpeedMultiplier;
    public PlayerAirborneState(PlayerController controller) : base(controller)
    {
        this.controller = controller;
    }
    public override void OnEnter()
    {
        controller.PlayerJump.SetJumpVelocity();
        Player.Instance.isGrounded= false;
        controller.Animator.SetBool(GroundedBoolHash, false);
        // 점프 상태 진입 시 실행될 코드 추가
        // 1. 점프 직전(지상)의 이동 입력 및 상태 캐싱 (공중 관성 고정)
        Vector3 lastInput = controller.moveInput;
        initialIsRun = controller.IsRunPressed;

        if (lastInput.sqrMagnitude > 0.001f)
        {
            float inputMagnitude = lastInput.magnitude;

            if (controller.CurrentDevice == PlayerController.ControlDeviceType.Gamepad)
            {
                float maxCap = initialIsRun ? 1.0f : 0.5f;
                initialSpeedMultiplier = Mathf.Clamp(inputMagnitude, 0f, maxCap);
            }
            else
            {
                initialSpeedMultiplier = initialIsRun ? 1.0f : 0.5f;
            }

            initialJumpDirection = controller.PlayerJump.CalculateDirection(lastInput);
        }
        else
        {
            initialSpeedMultiplier = 0f;
            initialJumpDirection = Vector3.zero;
        }

        // 2. 캐싱된 속도 수치(JumpSpeed)를 애니메이터에 설정하여 Sub-State Machine의 진입 상태 결정
        // (0.0 -> Jump_Idle, 0.5 -> Jump_Walk, 1.0 -> Jump_Run)
        controller.Animator.SetFloat(JumpSpeedHash, initialSpeedMultiplier);

        // 3. Sub-State Machine으로 CrossFade 진입
        //controller.Animator.CrossFade(AirborneSubStateHash, 0.1f);
        controller.Animator.SetTrigger(JumpTriggerHash);
    }
    public override void Update()
    {
        
        // Implement the logic for the Airborne state here
        //Debug.Log("PlayerAirborneState Update");

        //Debug.Log(moveInput);
        controller.PlayerJump.ExecuteJump(initialJumpDirection, initialIsRun);
        if (controller.PlayerJump.GetJumpVelocity() <= 0f && controller.PlayerJump.IsGround())
        {
            controller.MovementFSM.ChangeState(controller.locomotionState);
            return;
        }

    }
    public override void OnExit()
    {
        //Debug.Log("PlayerAirborneState OnExit");
        Player.Instance.isGrounded = true;
        controller.Animator.SetBool(GroundedBoolHash, true);
        // 점프 상태 종료 시 실행될 코드 추가
        // 상태 탈출 시 애니메이션 축 초기화
    }
}
