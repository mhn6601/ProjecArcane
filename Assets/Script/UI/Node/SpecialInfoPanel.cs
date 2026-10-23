using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스페셜 슬롯 클릭 시 열리는 정보 패널.
/// 슬롯 설명 / 현재 장착 노드 / 해제 버튼 / 후보 노드 / 장착(교체) 버튼.
/// 후보 노드는 스페셜 노드 인벤토리에서 클릭해서 선택한다.
/// </summary>
public class SpecialInfoPanel : MonoBehaviour
{
    [Header("슬롯 정보")]
    [SerializeField] private TMP_Text currentequippedNameText;   // 현재 장착 노드 이름
    [SerializeField] private TMP_Text currentequippedDescText;       // 슬롯 설명
    [SerializeField] private Image equippedIcon;

    [Header("후보 정보")]
    [SerializeField] private TMP_Text candidateNameText;
    [SerializeField] private TMP_Text candidateDescText;
    [SerializeField] private TMP_Text candidateInfoText;  // 장착 불가 사유 등
    [SerializeField] private Image candidateIcon;

    [Header("버튼")]
    [SerializeField] private Button equipButton;
    [SerializeField] private TMP_Text equipButtonText;
    [SerializeField] private Button unequipButton;
    [SerializeField] private TMP_Text unequipButtonText;

    private SpellData spell;
    private int slotIndex = -1;
    private SpecialNodeInstance candidate;

    private void Awake()
    {
        equipButton.onClick.AddListener(OnEquipClicked);
        unequipButton.onClick.AddListener(OnUnequipClicked);
    }

    private void OnEnable()
    {
        if (SpecialNodeInventoryManager.Instance != null)
            SpecialNodeInventoryManager.Instance.NodeSelected += OnCandidateSelected;

        if (SpellProgressManager.Instance != null)
            SpellProgressManager.Instance.OnSpecialChanged += Refresh;
    }

    private void OnDisable()
    {
        if (SpecialNodeInventoryManager.Instance != null)
            SpecialNodeInventoryManager.Instance.NodeSelected -= OnCandidateSelected;

        if (SpellProgressManager.Instance != null)
            SpellProgressManager.Instance.OnSpecialChanged -= Refresh;
    }

    /// <summary>
    /// 스페셜 슬롯 클릭 시 호출. 같은 슬롯을 다시 누르면 닫는다(토글).
    /// </summary>
    public void Toggle(SpellData targetSpell, int targetSlotIndex, string slotDescription)
    {
        if (targetSpell == null) return;

        if (gameObject.activeSelf && spell == targetSpell && slotIndex == targetSlotIndex)
        {
            Close();
            return;
        }

        spell = targetSpell;
        slotIndex = targetSlotIndex;
        candidate = null;

        if (currentequippedDescText != null) currentequippedDescText.text = slotDescription;

        gameObject.SetActive(true);
        Refresh();
    }

    public void Close()
    {
        candidate = null;
        gameObject.SetActive(false);
    }

    private void OnCandidateSelected(SpecialNodeInstance instance)
    {
        candidate = instance;
        Refresh();
    }

    private void Refresh()
    {
        if (spell == null || slotIndex < 0) return;

        var progress = SpellProgressManager.Instance;
        if (progress == null) return;

        // ---- 현재 장착 노드 ----
        SpecialNodeData equipped = progress.GetSpecialSlotData(spell.spellId, slotIndex);

        currentequippedNameText.text = equipped != null ? equipped.nodeName : "비어 있음";
        currentequippedDescText.text = equipped != null ? equipped.description : "";
        equippedIcon.enabled = equipped != null;
        if (equipped != null) equippedIcon.sprite = equipped.icon;
        unequipButton.interactable = equipped != null;

        // ---- 후보 노드 ----
        bool hasCandidate = candidate != null && candidate.Data != null;
        string candidateId = hasCandidate ? candidate.Data.nodeId : null;

        candidateNameText.text = hasCandidate ? candidate.Data.nodeName : "";
        candidateDescText.text = hasCandidate ? candidate.Data.description : "";
        candidateIcon.enabled = hasCandidate;
        if (hasCandidate) candidateIcon.sprite = candidate.Data.icon;

        bool canApply = hasCandidate && progress.CanEquipSpecial(spell.spellId, candidateId);
        bool alreadyHere = hasCandidate && equipped != null && equipped.nodeId == candidateId;
        bool elsewhere = hasCandidate && !alreadyHere
                         && progress.FindSpecialLocation(candidateId, out _, out _);

        if (!hasCandidate)
            candidateInfoText.text = "";
        else if (!canApply)
            candidateInfoText.text = candidate.Data.GetRequirementText();
        else if (alreadyHere)
            candidateInfoText.text = "현재 장착 중";
        else if (elsewhere)
            candidateInfoText.text = "다른 슬롯에 장착 중 (위치가 교환됩니다)";
        else
            candidateInfoText.text = "";

        equipButtonText.text = equipped != null ? "교체" : "장착";
        equipButton.interactable = canApply && !alreadyHere;
        unequipButtonText.text ="해제";
        unequipButton.interactable = equipped;
    }

    public void OnEquipClicked()
    {
        if (candidate == null || candidate.Data == null || spell == null) return;

        bool ok = SpellProgressManager.Instance.EquipOrSwapSpecial(
            spell.spellId, slotIndex, candidate.Data.nodeId);

        if (ok)
            candidate = null;   // 장착 후 후보 초기화 (OnSpecialChanged → Refresh)
        else
            Debug.LogWarning("[SpecialSlotInfoPanel] 장착 실패 (보유/태그/교환 조건 확인)");
    }

    private void OnUnequipClicked()
    {
        if (spell == null) return;
        SpellProgressManager.Instance.UnequipSpecial(spell.spellId, slotIndex);
    }
}
