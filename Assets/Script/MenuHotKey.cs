using UnityEngine;
using UnityEngine.InputSystem;

public class MenuHotkey : MonoBehaviour
{
    private InputAction escapeAction;

    private void Awake()
    {
        escapeAction = new InputAction("Escape", InputActionType.Button, "<Keyboard>/escape");
    }

    private void OnEnable()
    {
        escapeAction.performed += OnEscape;
        escapeAction.Enable();
    }

    private void OnDisable()
    {
        escapeAction.performed -= OnEscape;
        escapeAction.Disable();
    }

    private void OnDestroy() => escapeAction.Dispose();

    private void OnEscape(InputAction.CallbackContext ctx)
    {
        UIManager.Instance.HandleEscape();
    }
}