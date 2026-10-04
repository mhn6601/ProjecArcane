using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SpellNodeType
{
    Core,
    Spell,
    Stat,
    Special
}
public enum NodeState
{
    Locked,       // 부모 미해금 → 해금 불가
    Unlockable,   // 부모 해금됨 → 해금 가능
    Unlocked      // 이미 해금됨
}
public abstract class SpellNodeBase : MonoBehaviour
{
    [Header("ChildNode Connections")]
   
    public List<Transform> childNodes = new List<Transform>(); // 1:N 자식 노드 연결

    [Header("Node Info")]
    protected RectTransform rectTransform;
    [SerializeField] protected SpellNodeType nodeType;

    public SpellNodeBase parentNode;
    // 저장 데이터에서 사용하는 ID. 해금 개념이 없는 노드(스페셜)는 빈 문자열
    public virtual string NodeId => string.Empty;
    public bool IsUnlocked
    {
        get
        {
            var progress = SpellProgressManager.Instance;
            return progress != null
                && !string.IsNullOrEmpty(NodeId)
                && progress.IsUnlocked(NodeId, nodeType);
        }
    }
    public bool IsParentUnlocked {
        get 
        {
            if (parentNode == null) return true;                          // 루트
            if (parentNode.nodeType == SpellNodeType.Core) return true;   // 코어 노드는 항상 해금 취급
            return parentNode.IsUnlocked;
        }
    }
    public bool CanUnlock => !IsUnlocked && IsParentUnlocked;
    
    public NodeState State
    {
        get
        {
            if (IsUnlocked) return NodeState.Unlocked;
            return IsParentUnlocked ? NodeState.Unlockable : NodeState.Locked;
        }
    }
    public RectTransform RectTransform
    {
        get
        {
            if (rectTransform == null)
                rectTransform = GetComponent<RectTransform>();
            return rectTransform;
        }
    }
    // 해금 이벤트가 "나" 또는 "내 부모"의 것일 때만 다시 그리기 위한 판별
    protected bool IsRelated(string unlockedId)
    {
        if (string.IsNullOrEmpty(unlockedId)) return false;
        if (unlockedId == NodeId) return true;
        return parentNode != null && unlockedId == parentNode.NodeId;
    }

    // 해금 버튼 / 노드 클릭에서 호출. 조건을 코드에서도 강제함
    public bool TryUnlock()
    {
        if (!CanUnlock) return false;

        // 스킬 포인트 등 비용이 있다면 여기서 먼저 검사하고 차감
        return SpellProgressManager.Instance.Unlock(NodeId, nodeType);
    }
    protected virtual void OnEnable()
    {
        BindProgress();
        RefreshVisual();
    }

    protected virtual void OnDisable()
    {
        UnbindProgress();
    }
    // 자식이 SpellProgressManager 이벤트 구독/해제를 구현
    protected abstract void BindProgress();
    protected abstract void UnbindProgress();

    // 자식이 현재 상태에 맞게 잠금 아이콘, 흑백, 장착 표시 등을 갱신
    protected abstract void RefreshVisual();
}