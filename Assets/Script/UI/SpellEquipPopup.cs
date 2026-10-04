using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellEquipPopup : MonoBehaviour
{
    [SerializeField] private GameObject root;                 // 팝업 본체 (딤 배경 포함)
    [SerializeField] private SpellSlot[] slots;               // 팝업 안의 슬롯 4개 (slotIndex 0~3)
    [SerializeField] private TextMeshProUGUI titleText;       // "에너지볼트 - 장착할 슬롯 선택"
    [SerializeField] private Button unequipButton;            // 장착 해제 (선택)
    [SerializeField] private Button closeButton;              // 닫기/취소

    private SpellData target;

    public bool IsOpen => root.activeSelf;

    private void Awake()
    {
        foreach (var slot in slots) slot.OnClicked += HandleSlotClicked;

        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (unequipButton != null) unequipButton.onClick.AddListener(HandleUnequip);

        root.SetActive(false);
    }

    private void OnDestroy()
    {
        foreach (var slot in slots)
        {
            if (slot != null) slot.OnClicked -= HandleSlotClicked;
        }
    }

    public void Open(SpellData spell)
    {
        if (spell == null) return;

        // 팝업 오브젝트 자체가 꺼져 있으면 켬 (Awake는 이 시점에 실행됨)
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        // 같은 부모 아래 형제들 중 가장 위에 그려지게 함
        transform.SetAsLastSibling();

        target = spell;
        root.SetActive(true);

        Debug.Log($"[Equip] 3. Popup.Open / spell={spell.spellName} / root.activeInHierarchy={root.activeInHierarchy}");

        if (titleText != null) titleText.text = $"{spell.spellName} - 장착할 슬롯 선택";

        int equippedSlot = SpellProgressManager.Instance.FindSlot(spell.spellId);
        foreach (var slot in slots) slot.SetHighlight(slot.SlotIndex == equippedSlot);

        if (unequipButton != null) unequipButton.gameObject.SetActive(equippedSlot >= 0);
    }
    public void Close()
    {
        target = null;
        root.SetActive(false);
    }

    private void HandleSlotClicked(SpellSlot slot)
    {
        if (target == null) return;

        SpellProgressManager.Instance.Equip(slot.SlotIndex, target);
        Close();
    }

    private void HandleUnequip()
    {
        if (target == null) return;

        int equippedSlot = SpellProgressManager.Instance.FindSlot(target.spellId);
        if (equippedSlot >= 0) SpellProgressManager.Instance.Unequip(equippedSlot);
        Close();
    }
}