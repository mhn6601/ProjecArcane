using UnityEngine;
using UnityEngine.EventSystems;

public class SpecialNode : SpellNodeBase,IPointerClickHandler
{
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
        string spellId = targetNode.SpellId;   // SpellNode에 spellData.spellId 접근용 프로퍼티 추가

        bool targetUnlocked = progress.IsUnlocked(spellId, SpellNodeType.Spell);
        string equipped = progress.GetSpecialSlot(spellId, slotIndex);

        // targetUnlocked == false          → 슬롯 비활성(대상 스킬 미해금)
        // equipped가 비어 있음             → 빈 슬롯
        // equipped가 있음                  → 장착된 스페셜 아이콘 표시
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