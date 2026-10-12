using UnityEngine;

public static class GamePause
{
    private static int count;
    private static float prevTimeScale = 1f;
    private static CursorLockMode prevLockMode;
    private static bool prevCursorVisible;

    public static void Push()
    {
        if (count++ > 0) return;

        prevTimeScale = Time.timeScale;
        prevLockMode = Cursor.lockState;
        prevCursorVisible = Cursor.visible;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void Pop()
    {
        if (count == 0) return;
        if (--count > 0) return;

        Time.timeScale = prevTimeScale;
        Cursor.lockState = prevLockMode;
        Cursor.visible = prevCursorVisible;
    }
}