using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpecialInventoryEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text infoText;   // 장착 불가일 때 사유 표시
    [SerializeField] private Button button;

    public void Setup(SpecialNodeData node, bool usable, Action<SpecialNodeData> onClick)
    {
        nameText.text = node.nodeName;
        infoText.text = usable ? "" : node.GetRequirementText();
        button.interactable = usable;   // 태그가 안 맞는 노드는 선택 불가
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(node));
    }
}