using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpecialNode : SpellNodeBase,IPointerClickHandler
{
    [Header("Visual")]
    [SerializeField] private Image iconImage;        // 슬롯에 표시할 아이콘 Image
    [SerializeField] private Sprite emptySprite;     // 빈 슬롯 기본 이미지 (없으면 비워 두기)
    [SerializeField] private Color lockedColor = new Color(1f, 1f, 1f, 0.35f);

    [SerializeField] private SpellNode targetNode;
    [SerializeField] private int slotIndex;        // 이 노드가 대응하는 특수 슬롯 (0 또는 1)
    [SerializeField] private string specialId;     // 장착할 스페셜 아이템 ID (ScriptableObject가 있다면 그걸로 교체)

    protected override void BindProgress()
    {
        SpellProgressManager.Instance.OnSpecialChanged += HandleSpecialChanged;
        SpellProgressManager.Instance.OnSpellUnlocked += HandleSpellUnlocked;
    }

    protected override void UnbindProgress()
    {
        if (SpellProgressManager.Instance == null) return;

        SpellProgressManager.Instance.OnSpecialChanged -= HandleSpecialChanged;
        SpellProgressManager.Instance.OnSpellUnlocked -= HandleSpellUnlocked;
    }

    private void HandleSpellUnlocked(string id)
    {
        if (id == targetNode.SpellId) RefreshVisual();
    }
    private void HandleSpecialChanged() => RefreshVisual();
    protected override void RefreshVisual()
    {
        var progress = SpellProgressManager.Instance;
        if (progress == null || iconImage == null) return;

        string spellId = targetNode.SpellId;
        bool targetUnlocked = progress.IsUnlocked(spellId, SpellNodeType.Spell);

        SpecialNodeData equipped = progress.GetSpecialSlotData(spellId, slotIndex);

        if (equipped != null)
        {
            iconImage.sprite = equipped.icon;        // SpecialNodeData의 아이콘 필드명에 맞게
        }
        else
        {
            iconImage.sprite = emptySprite;
        }

        iconImage.enabled = iconImage.sprite != null;
        iconImage.color = targetUnlocked ? Color.white : lockedColor;   // 대상 스킬 미해금이면 흐리게
    }
    public string GetOwnerNodeID()
    {
        return targetNode.SpellId;
    }
    public int GetIndex()
    {
        return slotIndex;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭 이벤트 처리 로직
        SpellTreeManager.Instance.OnSpecialSlotClicked(this);
        SpellTreeManager.Instance.FocusNode((RectTransform)transform); 
    }
}