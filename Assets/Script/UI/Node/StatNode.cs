using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class StatNode : SpellNodeBase, IPointerClickHandler
{
   
    [SerializeField] private StatNodeData statData;

    [Header("Visual")]
    [SerializeField] private Image iconImage;                                   // 노드에 표시할 아이콘 Image
    [SerializeField] private StatIconSetSO iconSet;
    public StatIconType iconType;
    [SerializeField] private Color lockedColor = new Color(1f, 1f, 1f, 0.35f);
    [SerializeField] private Color unlockableColor = new Color(1f, 1f, 1f, 0.75f);
    [SerializeField] private Color unlockedColor = Color.white;

    public override string NodeId => statData.nodeId;

    protected override void BindProgress()
    {
        SpellProgressManager.Instance.OnSpellUnlocked += HandleUnlocked;
        RefreshVisual();                                                        // 시작 시 현재 상태 반영
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

        if (iconSet != null && statData != null)
            iconImage.sprite = iconSet.GetIcon(statData.iconType);


        iconImage.enabled = iconImage.sprite != null;

        switch (State)
        {
            case NodeState.Locked: iconImage.color = lockedColor; break;
            case NodeState.Unlockable: iconImage.color = unlockableColor; break;
            case NodeState.Unlocked: iconImage.color = unlockedColor; break;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SpellTreeManager.Instance.ToggleNodeInfoUI(statData, this);
        SpellTreeManager.Instance.FocusNode((RectTransform)transform);
    }
}