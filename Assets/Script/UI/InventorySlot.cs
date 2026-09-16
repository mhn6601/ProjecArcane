using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private ItemData itemData;
    [SerializeField]
    private Image itemImage;
    public void SetItem(ItemData item)
    {
        this.itemData = item;
        itemImage.gameObject.SetActive(true);

    }
    public void Clear()
    {
        itemData = null;
        itemImage.gameObject.SetActive(false);
    }
    public void OnClick()
    {
        if (itemData == null)
        {
            return;
            
        }
        bool success = InventoryManager.Instance.EquipItem(itemData as EquipmentItemData);
        if (success)
        {
            InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();
            inventoryUI.RefreshUI();
        }
    }
}
