using UnityEngine;

public class SpecialNodeUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private SpecialInfoPanel infoPanel;
    [SerializeField] private SpecialInventoryPanel inventoryPanel;

    private SpecialNode currentSlot;

    private bool IsOpen =>
        (infoPanel != null && infoPanel.gameObject.activeSelf) ||
        (inventoryPanel != null && inventoryPanel.gameObject.activeSelf);

    private void Awake()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.OnCloseClicked += CloseAll;
        }

        CloseAll();
    }

    private void OnDestroy()
    {
        if (inventoryPanel != null)
        {
            inventoryPanel.OnCloseClicked -= CloseAll;
        }
    }

    private void OnDisable()
    {
        CloseAll();
    }

    /// <summary>
    /// 스페셜 노드 슬롯을 클릭했을 때 호출됩니다.
    /// </summary>
    public void OnSlotClicked(SpecialNode slot)
    {
        if (slot == null)
            return;

        var spm = SpellProgressManager.Instance;

        if (spm == null)
        {
            Debug.LogError(
                "[SpecialNodeUIController] SpellProgressManager.Instance가 null입니다."
            );

            return;
        }

        string ownerNodeID = slot.GetOwnerNodeID();
        int slotIndex = slot.GetIndex();

        // 해당 스펠이 해금되어 있지 않으면 열지 않음
        if (!spm.IsUnlocked(ownerNodeID, SpellNodeType.Spell))
            return;

        SpellData spell = spm.GetSpellData(ownerNodeID);

        if (spell == null)
        {
            Debug.LogWarning(
                $"[SpecialNodeUIController] SpellData를 찾을 수 없습니다. " +
                $"ownerNodeID={ownerNodeID}"
            );

            return;
        }

        // 기존 패널 닫기
        CloseAll();

        currentSlot = slot;

        /*
         * InfoPanel
         *
         * 현재 SpecialInfoPanel의 구조에서는
         * 후보 선택 / 장착 / 교체 / 해제를
         * SpecialInfoPanel 자체가 담당합니다.
         */
        string slotDescription = GetSlotDescription(slot);

        infoPanel.Toggle(
            spell,
            slotIndex,
            slotDescription
        );

        /*
         * InventoryPanel
         *
         * 해당 슬롯에 장착할 수 있는 스페셜 노드 목록 표시
         */
        inventoryPanel.Show(
            slot.transform.position,
            spell,
            spm.GetAcquiredSpecials()
        );
    }

    /// <summary>
    /// 현재 스페셜 슬롯의 설명을 가져옵니다.
    /// 필요에 따라 SpecialNode에서 직접 가져오도록 변경하면 됩니다.
    /// </summary>
    private string GetSlotDescription(SpecialNode slot)
    {
        if (slot == null)
            return string.Empty;

        // SpecialNode에 설명 프로퍼티가 있다면 여기를 변경하세요.
        // 예:
        // return slot.Description;

        return string.Empty;
    }

    /// <summary>
    /// 모든 스페셜 관련 UI를 닫습니다.
    /// </summary>
    public void CloseAll()
    {
        if (infoPanel != null)
        {
            infoPanel.Close();
        }

        if (inventoryPanel != null)
        {
            inventoryPanel.Hide();
        }

        currentSlot = null;
    }
}