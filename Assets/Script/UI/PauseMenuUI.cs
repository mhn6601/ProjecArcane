using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : UIBase
{
    [SerializeField] private Button quitButton;
    [SerializeField] private Button openDebugButton;

    private void OnEnable()
    {
        quitButton.onClick.AddListener(QuitGame);
        openDebugButton.onClick.AddListener(OpenDebug);
    }

    private void OnDisable()
    {
        quitButton.onClick.RemoveListener(QuitGame);
        openDebugButton.onClick.RemoveListener(OpenDebug);
    }

    public override void Open()
    {
        base.Open();
        GamePause.Push();
    }

    public override void Close()
    {
        base.Close();
        GamePause.Pop();
    }

    private void OpenDebug()
    {
        UIManager.Instance.OpenUI(UIType.Debug);   // 메뉴는 자동으로 닫힘
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}