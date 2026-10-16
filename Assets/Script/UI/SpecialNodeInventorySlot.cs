using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class SpecialNodeInventorySlot : MonoBehaviour
{
    private SpecialNodeInstance specialNode;

    [SerializeField]
    private Image specialNodeImage;

    [SerializeField]
    private TMP_Text countText; // 선택: 비워두면 수량 표시 안 함

    public SpecialNodeInstance SpecialNode => specialNode;

    public void SetItem(SpecialNodeInstance instance)
    {
        specialNode = instance;
        specialNodeImage.sprite = instance.Data.icon;
        specialNodeImage.gameObject.SetActive(true);

        if (countText != null)
        {
            
            countText.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        specialNode = null;
        specialNodeImage.sprite = null;
        specialNodeImage.gameObject.SetActive(false);

        if (countText != null)
            countText.gameObject.SetActive(false);
    }

    public void OnClick()
    {
        Debug.Log(specialNode.Data.description);
        SpecialNodeInventoryManager.Instance.ShowInfo(specialNode);
    }
}