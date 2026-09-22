using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private EnemyResourceSystem resourceSystem;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas healthCanvas;
    [Header("Setting")]
    [SerializeField] private float hideDelay = 5f;
    private Camera mainCamera;
    private float hideTimer;
    private void Awake()
    {
        if (resourceSystem == null)
        {
            resourceSystem = GetComponentInParent<EnemyResourceSystem>();
            
        }
        mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        if (resourceSystem == null)
        {
            return;
        }
        resourceSystem.OnHealthChanged += HandleHealthChanged;
        resourceSystem.OnDeath += HandleDeath;
    }
    private void OnDisable()
    {
        if (resourceSystem == null)
        {
            return;
        }
        resourceSystem.OnHealthChanged -= HandleHealthChanged;
        resourceSystem.OnDeath -= HandleDeath;
    }
    private void Start()
    {
        healthSlider.maxValue = resourceSystem.MaxHealth;
        healthSlider.value = resourceSystem.CurrentHealth;
        healthCanvas.enabled = false;
    }
    private void Update()
    {
        if (!healthCanvas.enabled)
            return;

        LookAtCamera();

        hideTimer -= Time.deltaTime;

        if (hideTimer <= 0f)
        {
            healthCanvas.enabled = false;
        }
    }
    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        healthCanvas.enabled = true;

        hideTimer = hideDelay;
    }
    private void HandleDeath()
    {
        healthCanvas.enabled = false;
    }

    private void LookAtCamera()
    {
        if (mainCamera == null)
            return;

        transform.rotation = mainCamera.transform.rotation * Quaternion.Euler(0,180,0);
    }

}
