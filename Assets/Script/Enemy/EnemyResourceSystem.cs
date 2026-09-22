using System;
using UnityEngine;

public class EnemyResourceSystem : MonoBehaviour
{
    private EnemyStats enemyStats;

    public float MaxHealth => enemyStats.MaxHealth;
    public float MaxMana => enemyStats.MaxMana;
    public float MaxStamina => enemyStats.MaxStamina;
    public float StaminaRecoveryThreshold => enemyStats.staminaRecoveryThreshold;

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
        
        enemyStats = GetComponent<EnemyStats>();
        InitResources();
    }

    // Update is called once per frame
    void Update()
    {
        HandleStaminaRegeneration();
    }
    public void InitResources()
    {
        CurrentHealth = enemyStats.MaxHealth;
        CurrentMana = enemyStats.MaxMana;
        CurrentStamina = enemyStats.MaxStamina;
    }
    public void SetResources(float currentHealth, float currentMana, float currentStamina)
    {
        CurrentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
        CurrentMana = Mathf.Clamp(currentMana, 0f, MaxMana);
        CurrentStamina = Mathf.Clamp(currentStamina, 0f, MaxStamina);

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnManaChanged?.Invoke(CurrentMana, MaxMana);
        OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
    }
    // 체력
    public void TakeDamage(float damageAmount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damageAmount);
        OnHealthChanged?.Invoke(CurrentHealth, enemyStats.MaxHealth);

        if (CurrentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void HealHP(float amount)
    {
        CurrentHealth = Mathf.Min(enemyStats.MaxHealth, CurrentHealth + amount);
        OnHealthChanged?.Invoke(CurrentHealth, enemyStats.MaxHealth);
    }


    // 마나
    public bool UseMana(float amount)
    {
        if (CurrentMana < amount) return false;

        CurrentMana -= amount;
        OnManaChanged?.Invoke(CurrentMana, enemyStats.MaxMana);
        return true;
    }

    // 근접 공격, 패리, 처형 성공 시 호출하여 MP 수급
    public void AddMana(float amount)
    {
        CurrentMana = Mathf.Min(enemyStats.MaxMana, CurrentMana + amount);
        OnManaChanged?.Invoke(CurrentMana, enemyStats.MaxMana);
    }

    //스태미너
    public bool UseStamina(float amount)
    {
        if (CurrentStamina < amount) return false;

        CurrentStamina = Mathf.Max(0f, CurrentStamina - amount);
        lastStaminaUseTime = Time.time;
        OnStaminaChanged?.Invoke(CurrentStamina, enemyStats.MaxStamina);
        return true;
    }
    public bool UseStaminaEasy(float amount)
    {
        if (CurrentStamina < 1) return false;
        CurrentStamina = Mathf.Max(0f, CurrentStamina - amount);
        lastStaminaUseTime = Time.time;
        OnStaminaChanged?.Invoke(CurrentStamina, enemyStats.MaxStamina);
        return true;
    }


    public bool ConsumeStaminaContinous(float ratePerSecond)
    {
        if (IsStaminaExhausted) return false;

        float amountToConsume = ratePerSecond * Time.deltaTime;
        if (CurrentStamina < amountToConsume)
        {
            SetStaminaExhausted(true);
            CurrentStamina = 0f;
            lastStaminaUseTime = Time.time;
            OnStaminaChanged?.Invoke(CurrentStamina, MaxStamina);
            return false;
        }
        CurrentStamina -= amountToConsume;
        lastStaminaUseTime = Time.time;

        if (CurrentStamina <= 0f)
        {
            CurrentStamina = 0f;
            SetStaminaExhausted(true);
        }
        OnStaminaChanged?.Invoke(CurrentStamina, enemyStats.MaxStamina);
        return true;
    }
    private void HandleStaminaRegeneration()
    {
        // 부스트 조건 충족 여부에 따라 대기 시간과 회복 배율 결정
        bool isBoost = Time.time >= lastStaminaUseTime + enemyStats.staminaRegenBoostDelay;
        float currentDelay = isBoost ? enemyStats.staminaRegenBoostDelay : enemyStats.staminaRegenDelay;

        if (Time.time >= lastStaminaUseTime + currentDelay)
        {
            float boostMul = isBoost ? enemyStats.staminaRegenBoostMul : 1.0f;

            // 스태미너 회복 처리
            CurrentStamina = Mathf.Min(enemyStats.MaxStamina, CurrentStamina + enemyStats.staminaRegenRate * boostMul * Time.deltaTime);

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
}
