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
        // 스태미너 사용 후 대기 시간이 지났고, 아직 최대치가 아닐 때 회복
        if (Time.time >= lastStaminaUseTime + playerStats.staminaRegenDelay && CurrentStamina < MaxStamina)
        {
            CurrentStamina = Mathf.Min(playerStats.MaxStamina, CurrentStamina + playerStats.staminaRegenRate * Time.deltaTime);

            // 고갈 상태였고, 일정 수치 이상 회복되었다면 잠금 해제!
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
