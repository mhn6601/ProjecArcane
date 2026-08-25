using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : CharacterControllerBase
{
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private PlayerJump playerJump;


    private PlayerInputAction inputActions;
    public PlayerInputAction InputActions => inputActions;


    public Vector3 moveInput { get; private set; }


    #region StateName
    [Header("State Names")]
    [SerializeField] private string walkStateName;
    [SerializeField] private string idleStateName;
    [SerializeField] private string JumpStateName;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        inputActions = new PlayerInputAction();
        if (playerMove == null) playerMove = GetComponent<PlayerMove>();
        if (playerJump == null) playerJump = GetComponent<PlayerJump>();
    }
    private void OnEnable()
    {
        inputActions.PlayerAction.Enable();
        inputActions.PlayerAction.Move.performed += OnMovePerformed;
        inputActions.PlayerAction.Move.canceled += OnMoveCanceled;

        inputActions.PlayerAction.Jump.performed += OnJumpPerformed;
    }
    private void OnDisable()
    {
        inputActions.PlayerAction.Move.performed -= OnMovePerformed;
        inputActions.PlayerAction.Move.canceled -= OnMoveCanceled;

        inputActions.PlayerAction.Jump.performed -= OnJumpPerformed;

        inputActions.PlayerAction.Disable();
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {

        moveInput = context.ReadValue<Vector3>();
        ChangeState(walkStateName);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector3.zero;
        playerMove.ResetCache();// 입력 취소 시 캐시 방향 초기화

        ChangeState(idleStateName);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        //Debug.Log(playerJump.IsGround());
        if (playerJump.IsGround())
        {
            playerJump.Jump();
            ChangeState(JumpStateName);
        }
    }
    public Vector3 GetJumpVelocity()
    {
        return playerJump.GetJumpVelocity();
    }

}