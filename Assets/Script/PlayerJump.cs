using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerJump : MonoBehaviour
{
    [Header("GroundCheck")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float rayDistance=0.2f;
    private float gravity =-9.81f;
    private Vector3 jumpVleocity;

    private Vector3 jumpInput;

    [SerializeField]
    private float jumpHeight = 0f;

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
        inputActions.PlayerAction.Jump.performed += OnJumpPerformed;
    }
    private void OnDisable()
    {

        inputActions.PlayerAction.Jump.performed -= OnJumpPerformed;
        inputActions.PlayerAction.Disable();
    }

    private void FixedUpdate()
    {

        if (IsGround() && jumpVleocity.y < 0)
        {
            jumpVleocity.y = -8f;
        }

        jumpVleocity.y += gravity * Time.deltaTime;
        characterController.Move(jumpVleocity* Time.deltaTime);
    }

    private bool IsGround()
    {
        Vector3 ray = transform.position + new Vector3(0f, characterController.center.y-(characterController.height*0.5f)+ characterController.skinWidth,0f);
        return Physics.Raycast(ray, Vector3.down, rayDistance, groundLayer);
    }
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (IsGround())
        {
            jumpVleocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (characterController == null) characterController = GetComponent<CharacterController>();
        if (characterController == null) return;

        float feetOffsetY = characterController.center.y - (characterController.height * 0.5f);
        float skinOffset = characterController.skinWidth;
        Vector3 rayOrigin = transform.position + new Vector3(0f, feetOffsetY + skinOffset, 0f);
        float totalRayDistance = skinOffset + 0.15f;

        Gizmos.color = IsGround() ? Color.green : Color.red;
        Gizmos.DrawRay(rayOrigin, Vector3.down * totalRayDistance);
    }
}
