using UnityEngine;

// UI 패널 루트에 붙이면 패널이 켜져 있는 동안 커서가 표시된다
public class CursorUnlocker : MonoBehaviour
{
    private void OnEnable()
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.Request(this);
    }

    private void OnDisable()
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.Release(this);
    }
}