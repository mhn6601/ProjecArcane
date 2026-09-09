using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerResourceSystem : MonoBehaviour
{
    private PlayerStats playerStats;

    public float MaxHealth => playerStats.MaxHealth;
    public float MaxMana => playerStats.MaxMana;
    public float MaxStamina => playerStats.MaxStamina;

    public float CurrentHealth { get; private set; }
    public float CurrentMana { get; private set; }
    public float CurrentStamina { get; private set; }
    [Header("Stamina Settings")]
    [SerializeField] private float staminaRegenRate = 5f; // 초당 회복량
    [SerializeField] private float staminaRegenDelay = 4f; // 사용후 회복 시작 대기시간
    private float lastStaminaUseTime;
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<float, float> OnStaminaChanged;
    public event Action OnDeath;

    private void Awake()
    {
        playerStats = Player.Instance.Stats;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitResources();
    }

    // Update is called once per frame
    void Update()
    {
        RegenerateStamina();
    }
    public void InitResources()
    {
        CurrentHealth = playerStats.MaxHealth;
        CurrentMana = playerStats.MaxMana;
        CurrentStamina = playerStats.MaxStamina;

        NotifyAllUI();
    }

    // 체력
    public void TakeDamage(float damageAmount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damageAmount);
        OnHealthChanged?.Invoke(CurrentHealth, playerStats.MaxHealth);

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void HealHP(float amount)
    {
        CurrentHealth = Mathf.Min(playerStats.MaxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, playerStats.MaxHealth);
    }


    // 마나
    public bool UseMana(float amount)
    {
        if (CurrentMana < amount) return false;

        CurrentMana -= amount;
        OnManaChanged?.Invoke(CurrentMana, playerStats.MaxMana);
        return true;
    }

    // 근접 공격, 패리, 처형 성공 시 호출하여 MP 수급
    public void AddMana(float amount)
    {
        CurrentMana = Mathf.Min(playerStats.MaxMana, CurrentMana + amount);
        OnManaChanged?.Invoke(CurrentMana, playerStats.MaxMana);
    }

    //스태미너
    public bool UseStamina(float amount)
    {
        if (CurrentStamina < amount) return false;

        CurrentStamina = Mathf.Max(0f, CurrentStamina - amount);
        lastStaminaUseTime = Time.time;
        OnStaminaChanged?.Invoke(CurrentStamina, playerStats.MaxStamina);
        return true;
    }
    public bool UseStaminaEasy(float amount)
    {
        if(CurrentStamina <1) return false;
        CurrentStamina = Mathf.Max(0f, CurrentStamina - amount);
        lastStaminaUseTime = Time.time;
        OnStaminaChanged?.Invoke(CurrentStamina, playerStats.MaxStamina);
        return true;
    }

    private void RegenerateStamina()
    {
        if (Time.time < lastStaminaUseTime + staminaRegenDelay) return;
        if (CurrentStamina >= playerStats.MaxStamina) return;

        CurrentStamina = Mathf.Min(playerStats.MaxStamina, CurrentStamina + staminaRegenRate * Time.deltaTime);
        OnStaminaChanged?.Invoke(CurrentStamina, playerStats.MaxStamina);
    }
    public bool ConsumeStaminaContinous(float ratePerSecond)
    {
        float amountToConsume = ratePerSecond * Time.deltaTime;
        if (CurrentStamina < amountToConsume) return false;
        CurrentStamina = Mathf.Max(0f, CurrentStamina - amountToConsume);
        lastStaminaUseTime = Time.time;
        OnStaminaChanged?.Invoke(CurrentStamina, playerStats.MaxStamina);
        return true;
    }

    //기타
    private void HandleLevelUp(int newLevel)
    {

    }

    public void UpdateHealthUI(float currentHP, float maxHP)
    {
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }
    private void NotifyAllUI()
    {
        OnHealthChanged?.Invoke(CurrentHealth, playerStats.MaxHealth); 
        OnManaChanged?.Invoke(CurrentMana, playerStats.MaxMana);
        OnStaminaChanged?.Invoke(CurrentStamina, playerStats.MaxStamina);
    }
}
