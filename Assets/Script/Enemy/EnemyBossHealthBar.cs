using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private CanvasGroup canvasGroup;


    private EnemyResourceSystem resourceSystem;


    public void SetBoss(EnemyResourceSystem target)
    {
        // 기존 보스 이벤트 해제
        Unsubscribe();

        resourceSystem = target;

        if (resourceSystem == null)
        {
            Hide();
            return;
        }

        resourceSystem.OnHealthChanged += HandleHealthChanged;
        resourceSystem.OnDeath += HandleDeath;

        healthSlider.maxValue = resourceSystem.MaxHealth;
        healthSlider.value = resourceSystem.CurrentHealth;

        Show();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        Show();
    }

    private void HandleDeath()
    {
        Hide();
    }

    private void Show()
    {

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    private void Hide()
    {

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Unsubscribe()
    {
        if (resourceSystem == null)
            return;

        resourceSystem.OnHealthChanged -= HandleHealthChanged;
        resourceSystem.OnDeath -= HandleDeath;
    }
}