using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("슬롯 설정")]
    [SerializeField] private int slotIndex;           // 0~3 (키 1~4)

    [Header("UI 참조")]
    [SerializeField] private Image iconImage;         // 스펠 아이콘
    [SerializeField] private Image cooldownFill;      // Image Type = Filled (선택)
    [SerializeField] private TextMeshProUGUI keyText; // "1"~"4" (선택)
    [SerializeField] private TextMeshProUGUI nameText;  // 스펠 이름
    [SerializeField] private GameObject emptyVisual;  // 빈 슬롯 표시 (선택)
    [SerializeField] private GameObject highlight;    // 선택 중인 스펠이 이미 이 슬롯에 있음 표시 (선택)

    public int SlotIndex => slotIndex;
    public SpellData CurrentSpell { get; private set; }

    // 팝업에서 슬롯 클릭을 받기 위한 이벤트
    public event Action<SpellSlot> OnClicked;

    private void OnEnable()
    {
        var progress = SpellProgressManager.Instance;
        if (progress != null) progress.OnSlotChanged += HandleSlotChanged;

        RefreshSlot();
    }

    private void OnDisable()
    {
        if (SpellProgressManager.Instance != null)
            SpellProgressManager.Instance.OnSlotChanged -= HandleSlotChanged;
    }

    private void HandleSlotChanged(int changedIndex, SpellData spell)
    {
        if (changedIndex == slotIndex) RefreshSlot();
    }

    public void RefreshSlot()
    {
        var progress = SpellProgressManager.Instance;
        CurrentSpell = progress != null ? progress.GetEquipped(slotIndex) : null;

        bool hasSpell = CurrentSpell != null;

        iconImage.enabled = hasSpell;
        if (hasSpell) iconImage.sprite = CurrentSpell.Icon;

        if (nameText != null) nameText.text = CurrentSpell != null ? CurrentSpell.spellName : string.Empty;
        if (emptyVisual != null) emptyVisual.SetActive(!hasSpell);
        SetCooldown(0f);
    }

    // 쿨타임 표시: 1 = 막 사용함, 0 = 사용 가능
    public void SetCooldown(float normalized)
    {
        if (cooldownFill != null)
            cooldownFill.fillAmount = Mathf.Clamp01(normalized);
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