using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerDeathUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;            // 켜고 끌 자식 패널
    [SerializeField] private CanvasGroup canvasGroup;     // panel에 붙은 CanvasGroup
    [SerializeField] private Button respawnButton;
    [SerializeField] private float fadeDuration = 1.5f;

    private void Awake()
    {
        respawnButton.onClick.AddListener(OnRespawnClicked);
        panel.SetActive(false);
    }

    private void Start()
    {
        // Start에서 구독: GameManager가 Awake를 끝낸 뒤라 Instance가 안전함
        GameManager.Instance.PlayerDied += Show;
        GameManager.Instance.PlayerRespawned += Hide;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.PlayerDied -= Show;
        GameManager.Instance.PlayerRespawned -= Hide;
    }

    private void Show()
    {
        panel.SetActive(true);
        respawnButton.interactable = false;
        StartCoroutine(FadeIn());
    }

    private void Hide()
    {
        StopAllCoroutines();
        panel.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        respawnButton.interactable = true;
        EventSystem.current.SetSelectedGameObject(respawnButton.gameObject);
    }

    private void OnRespawnClicked()
    {
        respawnButton.interactable = false;
        GameManager.Instance.RespawnPlayer();
    }
}