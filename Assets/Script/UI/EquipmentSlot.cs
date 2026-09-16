using System;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField]
    private EquipmentSlotType itemType;
    public EquipmentSlotType ItemType => itemType;
    [SerializeField]
    private Image itemImage;
    private EquipmentItemData equipmentItemData;
    [SerializeField]
    private Transform iconTransform;
    [SerializeField]
    private Sprite idleSprite;


    private void OnEnable()
    {
        SetIdleIcon();
    }
    public void SetItem(EquipmentItemData equip)
    {
        this.equipmentItemData = equip;
        if (equip == null)
        {
            //itemImage.gameObject.SetActive(false);
            SetIdleIcon();
            return;
        }
        //itemImage.gameObject.SetActive(true);
    }
    public void Clear()
    {
        SetItem(null);
    }
    public void OnClick()
    {
        if (equipmentItemData == null)
        {
            return;
        }
        bool success = InventoryManager.Instance.UnEquipItem(equipmentItemData);
        if (success)
        {
            InventoryUI inventoryUI = GetComponentInParent<InventoryUI>();
            inventoryUI.RefreshUI();
        }
    }
    private void SetIdleIcon()
    {
        if (itemImage)
        {
            itemImage.sprite = idleSprite;
        }
        
    }

}
