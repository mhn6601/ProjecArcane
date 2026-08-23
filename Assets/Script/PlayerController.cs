using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : CharacterControllerBase
{
    [SerializeField] private PlayerMove playerMove;
    [SerializeField] private Animator animator;


    private PlayerInputAction inputActions;


    public PlayerInputAction InputActions=>inputActions;
    public Animator Animator => animator;
    public CharacterController CC => characterController;

    public Vector3 moveInput { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        inputActions = new PlayerInputAction();
        if (playerMove == null) playerMove = GetComponent<PlayerMove>();
    }
    private void OnEnable()
    {
        inputActions.PlayerAction.Enable();
        inputActions.PlayerAction.Move.performed += OnMovePerformed;
        inputActions.PlayerAction.Move.canceled += OnMoveCanceled;
    }
    private void OnDisable()
    {
        inputActions.PlayerAction.Move.performed -= OnMovePerformed;
        inputActions.PlayerAction.Move.canceled -= OnMoveCanceled;
        inputActions.PlayerAction.Disable();
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector3>();
        if (!Player.Instance.isWalking)
        {
            Player.Instance.isWalking = true;
        }
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector3.zero;
        playerMove.ResetCache();// 입력 취소 시 캐시 방향 초기화

        if (Player.Instance.isWalking)
        {
            Player.Instance.isWalking = false;
        }
    }
}
