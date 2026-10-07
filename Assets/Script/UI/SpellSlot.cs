using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("슬롯 설정")]
    [SerializeField] private int slotIndex;

    [Header("UI 참조")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject emptyVisual;
    [SerializeField] private GameObject highlight;

    public int SlotIndex => slotIndex;
    public SpellData CurrentSpell { get; private set; }
    public event Action<SpellSlot> OnClicked;

    // 쿨타임 진행 상태
    private float cooldownDuration;   // 총 쿨타임(초)
    private float cooldownRemaining;  // 남은 시간(초)

    private void OnEnable()
    {
        var progress = SpellProgressManager.Instance;
        if (progress != null) progress.OnSlotChanged += HandleSlotChanged;

        if (Player.Instance != null)
            Player.Instance.CastSpell.OnSpellCast += HandleSpellCast;

        RefreshSlot();
    }

    private void OnDisable()
    {
        if (SpellProgressManager.Instance != null)
            SpellProgressManager.Instance.OnSlotChanged -= HandleSlotChanged;

        if (Player.Instance != null)
            Player.Instance.CastSpell.OnSpellCast -= HandleSpellCast;
    }

    private void HandleSlotChanged(int changedIndex, SpellData spell)
    {
        if (changedIndex == slotIndex) RefreshSlot();
    }

    private void HandleSpellCast(int spell, float normalized)
    {
        if (spell != slotIndex) return;

        // normalized(보통 1) 기준으로 남은 시간 시작
        cooldownDuration = CurrentSpell != null ? CurrentSpell.cooldown : 0f; // ← SpellData의 쿨타임 필드명에 맞게 수정
        cooldownRemaining = cooldownDuration * Mathf.Clamp01(normalized);
        UpdateCooldownUI();
    }

    private void Update()
    {
        if (cooldownRemaining <= 0f) return;

        cooldownRemaining = Mathf.Max(0f, cooldownRemaining - Time.deltaTime);
        UpdateCooldownUI();
    }

    private void UpdateCooldownUI()
    {
        float normalized = cooldownDuration > 0f ? cooldownRemaining / cooldownDuration : 0f;

        if (cooldownFill != null)
            cooldownFill.fillAmount = normalized;

        if (cooldownText != null)
            cooldownText.text = cooldownRemaining > 0f
                ? Mathf.CeilToInt(cooldownRemaining).ToString()
                : string.Empty;
    }

    public void RefreshSlot()
    {
        var progress = SpellProgressManager.Instance;
        CurrentSpell = progress != null ? progress.GetEquipped(slotIndex) : null;

        bool hasSpell = CurrentSpell != null;

        iconImage.enabled = hasSpell;
        if (hasSpell) iconImage.sprite = CurrentSpell.Icon;

        if (nameText != null) nameText.text = hasSpell ? CurrentSpell.spellName : string.Empty;
        if (emptyVisual != null) emptyVisual.SetActive(!hasSpell);

        cooldownRemaining = 0f;
        UpdateCooldownUI();
    }

    public void SetHighlight(bool on)
    {
        if (highlight != null) highlight.SetActive(on);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke(this);
    }
}