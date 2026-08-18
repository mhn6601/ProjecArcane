using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMove:MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    private Vector3 moveVleocity;

    private Vector3 moveInput;
    private CharacterController characterController;

    private PlayerInputAction inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
        characterController = GetComponent<CharacterController>();
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

    private void FixedUpdate()
    {
        moveVleocity = moveInput * moveSpeed*Time.deltaTime;
        
        characterController.Move(moveVleocity);
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector3>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector3.zero;
    }
    private void CharacterRotate()
    {

    }
}
