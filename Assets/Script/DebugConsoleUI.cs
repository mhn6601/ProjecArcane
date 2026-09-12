using System;
using System.Diagnostics;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsoleUI : MonoBehaviour
{
    [Header("UI Component References")]
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Settings")]
    [SerializeField] private int maxLogCount = 50; // 메모리 보호를 위한 최대 로그 보관 개수
    [SerializeField] private bool showStackTrace = false; // 에러 발생 시 스택 트레이스 표시 여부
    private readonly System.Collections.Generic.Queue<string> logQueue = new System.Collections.Generic.Queue<string>();
    private readonly StringBuilder logStringBuilder = new StringBuilder();

    private void OnEnable()
    {
        // 유니티 로그 이벤트 구독
        Application.logMessageReceived += HandleLog;
    }
    private void OnDisable()
    {
        // 이벤트 해제 (메모리 누수 방지)
        Application.logMessageReceived -= HandleLog;
    }
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // 로그 유형별 색상 처리
        string colorHex = type switch
        {
            LogType.Error or LogType.Exception => "#FF4D4D", // 빨간색
            LogType.Warning => "#FFCC00",                   // 노란색
            _ => "#FFFFFF"                                   // 흰색
        };

        string formattedLog = $"<color={colorHex}>[{type}] {logString}</color>";

        if ((type == LogType.Error || type == LogType.Exception) && showStackTrace)
        {
            formattedLog += $"\n<color=#888888>{stackTrace}</color>";
        }

        // 큐 관리 (최대 로그 개수 초과 시 오래된 로그 제거)
        logQueue.Enqueue(formattedLog);
        if (logQueue.Count > maxLogCount)
        {
            logQueue.Dequeue();
        }

        // 전체 텍스트 조합
        logStringBuilder.Clear();
        foreach (var log in logQueue)
        {
            logStringBuilder.AppendLine(log);
        }

        logText.text = logStringBuilder.ToString();

        // 스크롤바 최하단으로 이동
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
    // 화면의 로그 초기화 기능 (버튼 이벤트 연결용)
    public void ClearLog()
    {
        logQueue.Clear();
        logStringBuilder.Clear();
        logText.text = string.Empty;
    }
}
