using System.Collections.Generic;
using UnityEngine;

public class SpellTreeManager : Singleton<SpellTreeManager>
{
    Player player;
    [SerializeField] private Transform contentParent;            // ScrollView의 Content
    [SerializeField] private SpellNodeLineRenderer lineRenderer; // 연결선 렌더러
    [SerializeField] private NodeInfoUI nodeInfoUI;              // 노드 정보 UI
    [SerializeField] private SpellEquipPopup equipPopup;

    private SpellData currentSpell;
    private StatNodeData currentStat;
    private SpellNodeBase currentNode;                           // 현재 정보 패널에 표시 중인 노드
    private List<SpellNodeBase> allNodes = new List<SpellNodeBase>();

    private void Start()
    {
        RefreshSkillTreeLines();
        nodeInfoUI.OnEquipClicked += HandleEquipClicked;
        nodeInfoUI.OnUnlockClicked += HandleUnlockClicked;

        var progress = SpellProgressManager.Instance;
        if (progress != null)
        {
            progress.OnSpellUnlocked += HandleProgressChanged;
            progress.OnSlotChanged += HandleSlotChanged;
        }
    }

    private void OnEnable()
    {
        player = Player.Instance;
        player.Stats.OnStatsChanged += OnStatsChanged;
    }

    private void OnDisable()
    {
        if (player != null) player.Stats.OnStatsChanged -= OnStatsChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (nodeInfoUI != null)
        {
            nodeInfoUI.OnEquipClicked -= HandleEquipClicked;
            nodeInfoUI.OnUnlockClicked -= HandleUnlockClicked;
        }

        if (SpellProgressManager.Instance != null)
        {
            SpellProgressManager.Instance.OnSpellUnlocked -= HandleProgressChanged;
            SpellProgressManager.Instance.OnSlotChanged -= HandleSlotChanged;
        }
    }

    /// <summary>
    /// Content 하위의 모든 노드를 자동 스캔하여 선을 다시 그려줌
    /// </summary>
    public void RefreshSkillTreeLines()
    {
        allNodes.Clear();

        // 1. Content 하위의 모든 SpellNode 컴포넌트를 자동으로 수집
        SpellNodeBase[] nodes = contentParent.GetComponentsInChildren<SpellNodeBase>();
        allNodes.AddRange(nodes);

        // 2. LineRenderer에 노드 리스트 전달 (단 1개의 DrawCall로 렌더링)
        if (lineRenderer != null)
        {
            lineRenderer.SetLines(allNodes);
        }
    }

    // 인스펙터 우측 상단 Context Menu(점 3개)나 스크립트 우클릭 시 실행 가능
    [ContextMenu("Refresh Spell Tree Lines")]
    public void RefreshSpellTreeLines()
    {
        if (contentParent == null || lineRenderer == null) return;

        List<SpellNodeBase> allNodes = new List<SpellNodeBase>();
        SpellNodeBase[] nodes = contentParent.GetComponentsInChildren<SpellNodeBase>();
        allNodes.AddRange(nodes);

        lineRenderer.SetLines(allNodes);
    }

    private void OnValidate()
    {
        // 인스펙터 수치가 변경될 때마다 에디터에서 즉시 선 갱신
        RefreshSpellTreeLines();
    }

    // -------------------------
    // 정보 패널 열기 (노드 참조를 함께 받음)
    // -------------------------
    public void ToggleNodeInfoUI(SpellData spellData, SpellNodeBase node)
    {
        currentStat = null;
        currentSpell = spellData;
        currentNode = node;

        if (spellData == null || node == null)
        {
            CloseNodeInfoUI();
            return;
        }

        if (!nodeInfoUI.isPanelActive())
            nodeInfoUI.ToggleNodeInfoPanel(true);

        nodeInfoUI.SetNodeInfo(
            spellData.spellName,
            SpellTooltipFormatter.Format(spellData, Player.Instance.Stats),
            spellData.VideoClip);

        RefreshActionButtons();
    }

    public void ToggleNodeInfoUI(StatNodeData statData, SpellNodeBase node)
    {
        currentSpell = null;
        currentStat = statData;
        currentNode = node;

        if (statData == null || node == null)
        {
            CloseNodeInfoUI();
            return;
        }

        if (!nodeInfoUI.isPanelActive())
            nodeInfoUI.ToggleNodeInfoPanel(true);

        nodeInfoUI.SetNodeInfo(
            statData.nodeName,
            StatTooltipFormatter.Format(statData, Player.Instance.Stats),
            null);

        RefreshActionButtons();
    }

    public void CloseNodeInfoUI()
    {
        currentSpell = null;
        currentStat = null;
        currentNode = null;

        equipPopup.Close();
        nodeInfoUI.ToggleNodeInfoPanel(false);
    }

    // -------------------------
    // 해금/장착 버튼 상태 갱신 (노드의 실제 상태 기준)
    // -------------------------
    private void RefreshActionButtons()
    {
        //Debug.Log($"[RefreshActionButtons] node={currentNode?.name}, state={currentNode?.State}, canUnlock={currentNode?.CanUnlock}");
        if (currentNode == null)
        {
            nodeInfoUI.SetUnlockButton(false);
            nodeInfoUI.SetEquipButton(false);
            return;
        }

        bool unlocked = currentNode.IsUnlocked;

        // 해금 버튼: 해금 완료면 숨김 / 부모 해금됨이면 활성 / 아니면 비활성 + 안내 문구
        if (unlocked)
        {
            nodeInfoUI.SetUnlockButton(false);
        }
        else if (currentNode.CanUnlock)
        {
            nodeInfoUI.SetUnlockButton(true, true, "해금");
        }
        else
        {
            nodeInfoUI.SetUnlockButton(true, false, "상위 노드를 먼저 해금하세요");
        }

        // 장착 버튼: 해금된 스펠만. 이미 장착 중이면 "슬롯 변경"
        if (currentSpell != null && unlocked)
        {
            bool equipped = SpellProgressManager.Instance.FindSlot(currentSpell.spellId) >= 0;
            nodeInfoUI.SetEquipButton(true, equipped ? "슬롯 변경" : "장착");
        }
        else
        {
            nodeInfoUI.SetEquipButton(false);   // 스탯 노드이거나 미해금
        }
    }

    // -------------------------
    // 이벤트 핸들러
    // -------------------------
    private void OnStatsChanged()
    {
        if (!nodeInfoUI.isPanelActive()) return;

        if (currentSpell != null)
        {
            nodeInfoUI.UpdateDescription(
                SpellTooltipFormatter.Format(currentSpell, Player.Instance.Stats));
        }
        else if (currentStat != null)
        {
            nodeInfoUI.UpdateDescription(
                StatTooltipFormatter.Format(currentStat, Player.Instance.Stats));
        }
    }

    // 어떤 노드든 해금되면 패널의 버튼 상태를 다시 계산 (부모 해금 → 자식 해금 가능으로 바뀌는 경우 포함)
    private void HandleProgressChanged(string unlockedId)
    {
        if (nodeInfoUI.isPanelActive()) RefreshActionButtons();
    }

    // 장착 슬롯이 바뀌면 "장착"/"슬롯 변경" 문구 갱신
    private void HandleSlotChanged(int slotIndex, SpellData spell)
    {
        if (nodeInfoUI.isPanelActive()) RefreshActionButtons();
    }

    private void HandleEquipClicked()
    {
        Debug.Log($"[Equip] 2. 매니저 수신 / currentSpell={(currentSpell != null ? currentSpell.spellName : "null")} / equipPopup={(equipPopup != null ? "연결됨" : "NULL")}");

        if (currentSpell != null) equipPopup.Open(currentSpell);
    }

    private void HandleUnlockClicked()
    {
        if (currentNode == null) return;

        // 부모 미해금이면 TryUnlock이 false를 반환 (버튼이 비활성이라 보통 도달하지 않는 이중 방어)
        // 성공하면 OnSpellUnlocked 이벤트 → HandleProgressChanged에서 버튼 상태가 갱신됨
        currentNode.TryUnlock();
    }
}