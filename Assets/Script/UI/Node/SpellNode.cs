using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellNode : SpellNodeBase, IPointerClickHandler
{
    [SerializeField] private SpellData spellData;
    [SerializeField] private Image iconImage;

    [Header("상태별 아이콘 색")]
    [SerializeField] private Color lockedColor = new Color(0.25f, 0.25f, 0.25f, 1f);
    [SerializeField] private Color unlockableColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color unlockedColor = Color.white;

    public override string NodeId => spellData.spellId;
    public string SpellId => spellData.spellId;

    protected override void BindProgress()
    {
        SpellProgressManager.Instance.OnSpellUnlocked += HandleUnlocked;
    }

    protected override void UnbindProgress()
    {
        if (SpellProgressManager.Instance != null)
            SpellProgressManager.Instance.OnSpellUnlocked -= HandleUnlocked;
    }

    private void HandleUnlocked(string id)
    {
        if (IsRelated(id)) RefreshVisual();
    }

    protected override void RefreshVisual()
    {
        if (iconImage == null) return;

        if (iconImage.sprite == null && spellData != null)
            iconImage.sprite = spellData.Icon;  
        iconImage.color = State switch
        {
            NodeState.Unlocked => unlockedColor,
            NodeState.Unlockable => unlockableColor,
            _ => lockedColor
        };
    }

    // 클릭 → 정보 패널 열기
    public void OnPointerClick(PointerEventData eventData)
    {
        // SpellNode.OnPointerClick
        SpellTreeManager.Instance.ToggleNodeInfoUI(spellData, this);

    }
}