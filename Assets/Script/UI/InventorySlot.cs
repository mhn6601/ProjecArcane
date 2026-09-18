using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]
    private ItemData itemData;
    [SerializeField]
    private Image itemImage;
    public void SetItem(ItemData item)
    {
        this.itemData = item;
        itemImage.sprite = itemData.icon;
        itemImage.gameObject.SetActive(true);
        

    }
    public void Clear()
    {
        itemData = null;
        itemImage.sprite = null;
        itemImage.gameObject.SetActive(false);
    }
    public void OnClick()
    {
        if (itemData == null)
            return;

        if (itemData is not EquipmentItemData equipmentItem)
            return;

        InventoryManager.Instance.EquipItem(equipmentItem);
    }
}
