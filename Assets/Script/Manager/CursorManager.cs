using System;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : Singleton<CursorManager>
{
    // 커서를 필요로 하는 UI 목록 (중복 등록 방지를 위해 HashSet)
    private readonly HashSet<object> requesters = new HashSet<object>();

    public bool IsCursorUnlocked => requesters.Count > 0;
    public event Action<bool> OnCursorStateChanged;   // true = 커서 보임 (카메라 입력 차단 등에 활용)

    private void Start() => Apply();

    // 창 포커스를 되찾을 때 상태 재적용 (Alt+Tab 후 잠금이 풀리는 문제 방지)
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) Apply();
    }

    public void Request(object owner)
    {
        if (requesters.Add(owner)) Apply();
    }

    public void Release(object owner)
    {
        if (requesters.Remove(owner)) Apply();
    }

    private void Apply()
    {
        bool unlocked = IsCursorUnlocked;
        Cursor.lockState = unlocked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = unlocked;
        OnCursorStateChanged?.Invoke(unlocked);
    }
}