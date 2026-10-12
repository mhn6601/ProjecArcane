using UnityEngine;
using UnityEngine.UI;
using TMPro; // 레거시 Text를 쓰면 UnityEngine.UI의 Text로 바꾸세요

public class InventorySlot : MonoBehaviour
{
    private ItemInstance item;

    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private TMP_Text countText; // 선택: 비워두면 수량 표시 안 함

    public ItemInstance Item => item;

    public void SetItem(ItemInstance instance)
    {
        item = instance;
        itemImage.sprite = instance.Data.icon;
        itemImage.gameObject.SetActive(true);

        if (countText != null)
        {
            if (instance is StackItemInstance stack && stack.count > 1)
            {
                countText.text = stack.count.ToString();
                countText.gameObject.SetActive(true);
            }
            else
            {
                countText.gameObject.SetActive(false);
            }
        }
    }

    public void Clear()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.gameObject.SetActive(false);

        if (countText != null)
            countText.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        if (item is not EquipmentInstance equipment)
            return;

        InventoryManager.Instance.EquipItem(equipment);
    }
}