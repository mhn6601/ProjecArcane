using UnityEngine;
using UnityEngine.EventSystems;

public class StatNode : SpellNodeBase,IPointerClickHandler
{
    [SerializeField] private StatNodeData statData;

    public override string NodeId => statData.nodeId;

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
        switch (State)
        {
            case NodeState.Locked:     /* 잠김 */ break;
            case NodeState.Unlockable: /* 해금 가능 */ break;
            case NodeState.Unlocked:   /* 해금 완료 */ break;
        }
    }
    // 클릭 → 정보 패널 열기
    public void OnPointerClick(PointerEventData eventData)
    {
        SpellTreeManager.Instance.ToggleNodeInfoUI(statData, this);
        SpellTreeManager.Instance.FocusNode((RectTransform)transform);
    }
}