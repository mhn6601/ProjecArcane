using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Resource Slider")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthBarText;
    [SerializeField] private Slider manaBar;
    [SerializeField] private TextMeshProUGUI manaBarText;
    [SerializeField] private Slider staminaBar;
    [SerializeField] private TextMeshProUGUI staminaBarText;

    [Header("Sub UI Views")]
    [SerializeField] private PlayerResourceSystem playerResourceSystem;
    [SerializeField] private CanvasGroup playerUICanvasGroup;
    [SerializeField] private PlayerResourceSystem targetplayerResourceSystem;

    private void Start()
    {
        // Player 메인 파사드를 통해 대상 참조 바인딩
        if (Player.Instance != null)
        {
            Init(Player.Instance);
        }
    }

    public void Init(Player player)
    {
        if (player.PlayerStats != null)
        {
            targetplayerResourceSystem = player.PlayerResourceSystem;

            // 이벤트 구독
            targetplayerResourceSystem.OnHealthChanged += HandleHealthChanged;
            targetplayerResourceSystem.OnManaChanged += HandleManaChanged;
            targetplayerResourceSystem.OnStaminaChanged += HandleStaminaChanged;
            targetplayerResourceSystem.OnDeath += HandlePlayerDeath;

            // 초기 UI 상태 동기화
            HandleHealthChanged(targetplayerResourceSystem.CurrentHealth, targetplayerResourceSystem.MaxHealth);
            HandleManaChanged(targetplayerResourceSystem.CurrentMana, targetplayerResourceSystem.MaxMana);
            HandleStaminaChanged(targetplayerResourceSystem.CurrentStamina, targetplayerResourceSystem.MaxStamina);
        }
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위한 이벤트 해제
        if (targetplayerResourceSystem != null)
        {
            targetplayerResourceSystem.OnHealthChanged -= HandleHealthChanged;
            targetplayerResourceSystem.OnManaChanged -= HandleManaChanged;
            targetplayerResourceSystem.OnStaminaChanged -= HandleStaminaChanged;
            targetplayerResourceSystem.OnDeath -= HandlePlayerDeath;
        }
    }

    private void HandleHealthChanged(float currentHP, float maxHP)
    {
        healthBar.value = currentHP / maxHP;
        healthBarText.text = $"{currentHP:F0} / {maxHP:F0}";
    }
    private void HandleManaChanged(float currentMP, float maxMP)
    {
        manaBar.value = currentMP / maxMP;
        manaBarText.text = $"{currentMP:F0} / {maxMP:F0}";
    }
    private void HandleStaminaChanged(float currentSP, float maxSP)
    {
        staminaBar.value = currentSP / maxSP;
        staminaBarText.text = $"{currentSP:F0} / {maxSP:F0}";   
    }

    private void HandlePlayerDeath()
    {
        // 사망 시 플레이어 UI 감추기 처리 etc.
        //if (playerUICanvasGroup != null)
        //{
        //    playerUICanvasGroup.alpha = 0.5f;
        //}
    }
}