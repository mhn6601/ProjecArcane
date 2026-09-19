using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerResourceSystem : MonoBehaviour
{
    private PlayerStats playerStats;

    public float MaxHealth => playerStats.MaxHealth;
    public float MaxMana => playerStats.MaxMana;
    public float MaxStamina => playerStats.MaxStamina;
    public float StaminaRecoveryThreshold => playerStats.staminaRecoveryThreshold;

    public float CurrentHealth { get; private set; }
    public float CurrentMana { get; private set; }
    public float CurrentStamina { get; private set; }
    private float lastStaminaUseTime;

    // 스태미너 고갈(잠금) 상태 여부 외부 참조용 프로퍼티
    public bool IsStaminaExhausted { get; private set; } = false;
    public event Action<float, float> OnHealthChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<float, float> OnStaminaChanged;
    public event Action<bool> OnStaminaExhaustedChanged;
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
        HandleStaminaRegeneration();
    }
    public void InitResources()
    {
        CurrentHealth = playerStats.MaxHealth;
        CurrentMana = playerStats.MaxMana;
        CurrentStamina = playerStats.MaxStamina;

        NotifyAllUI();
    }
    public void SetResources(float currentHealth,float currentMana, float currentStamina)
    {
        CurrentHealth = currentHealth;
        CurrentMana = currentMana;
        CurrentStamina = currentStamina;
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

    
    public bool ConsumeStaminaContinous(float ratePerSecond)
    {
        if(IsStaminaExhausted) return false;

        float amountToConsume = ratePerSecond * Time.deltaTime;
        if (CurrentStamina < amountToConsume) 
        {
            SetStaminaExhausted(true);
            CurrentStamina = 0f;
            lastStaminaUseTime = Time.time;
            OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
            return false;
        }
        CurrentStamina -=  amountToConsume;
        lastStaminaUseTime = Time.time;

        if (CurrentStamina <= 0f)
        {
            CurrentStamina = 0f;
            SetStaminaExhausted(true);
        }
        OnStaminaChanged?.Invoke(CurrentStamina, playerStats.MaxStamina);
        return true;
    }
    private void HandleStaminaRegeneration()
    {
        // 부스트 조건 충족 여부에 따라 대기 시간과 회복 배율 결정
        bool isBoost = Time.time >= lastStaminaUseTime + playerStats.staminaRegenBoostDelay;
        float currentDelay = isBoost ? playerStats.staminaRegenBoostDelay : playerStats.staminaRegenDelay;

        if (Time.time >= lastStaminaUseTime + currentDelay)
        {
            float boostMul = isBoost ? playerStats.staminaRegenBoostMul : 1.0f;

            // 스태미너 회복 처리
            CurrentStamina = Mathf.Min(playerStats.MaxStamina, CurrentStamina + playerStats.staminaRegenRate * boostMul * Time.deltaTime);

            // 고갈 상태 해제 체크
            if (IsStaminaExhausted && CurrentStamina >= StaminaRecoveryThreshold)
            {
                SetStaminaExhausted(false);
            }

            OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
        }
    }

    private void SetStaminaExhausted(bool exhausted)
    {
        if (IsStaminaExhausted != exhausted)
        {
            IsStaminaExhausted = exhausted;
            OnStaminaExhaustedChanged?.Invoke(IsStaminaExhausted);
            // Debug.Log(exhausted ? "스태미너 고갈! 달리기 잠금" : "스태미너 회복! 달리기 가능");
        }
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
