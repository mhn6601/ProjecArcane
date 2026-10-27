using System;
using System.Text.Json.Serialization;
using UnityEngine;

public  class Enemy : MonoBehaviour
{
    [Header("Sub Systems / Components")]
    [SerializeField] private  EnemyAnimation animation;
    [SerializeField] private EnemyController controller;
    [SerializeField] private EnemyStatus status;
    [SerializeField] private EnemyStats stats;
    [SerializeField] private EnemyResourceSystem resourceSystem;
    [SerializeField] private EnemyAttack attack;
    [SerializeField] private EnemyBrain brain;


    public EnemyAnimation Animation => animation;
    public EnemyController Controller=> controller;
    public EnemyStatus Status => status;
    public EnemyStats Stats=> stats;
    public EnemyResourceSystem ResourceSystem=> resourceSystem;
    public EnemyAttack Attack => attack;
    public bool isDead => resourceSystem != null && resourceSystem.IsDead;
    /// <summary>부활이 일어난 직후 호출 (UI/히트박스 복구 등 외부 구독용)</summary>
    public event Action OnRevived;

    private void Awake()
    {
        InitComponents();
    }
    private void Start()
    {
        if (ResourceSystem != null)
            ResourceSystem.OnDeath += HandleDeath;
    }
    protected  void OnDestroy()
    {

        if (ResourceSystem != null)
            ResourceSystem.OnDeath -= HandleDeath;
    }
    private void InitComponents()
    {
        if (animation == null)
        { animation = GetComponent<EnemyAnimation>();  }
        controller = GetComponent<EnemyController>();
        if (status == null) status = GetComponent<EnemyStatus>();
        if (stats == null) stats = GetComponent<EnemyStats>();
        if (resourceSystem == null) resourceSystem = GetComponent<EnemyResourceSystem>();
        if(attack == null) attack = GetComponent<EnemyAttack>();
    }
    private void HandleDeath()
    {
        controller.HandleDeath();
    }
    /// <summary>부활 허용 여부는 호출하는 쪽(EnemyRespawn)이 결정한다.</summary>
    public void Revive()
    {
        if (!isDead) return;

        resourceSystem.Revive();
        controller.HandleRevived();
        OnRevived?.Invoke();
    }
}
