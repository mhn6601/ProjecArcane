using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 착용 팝업: 노드 정보 + 착용/닫기 버튼
public class SpecialEquipPopup : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text extraText;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button closeButton;

    public event Action OnEquipClicked;
    public event Action OnCloseClicked;

    private void Awake()
    {
        equipButton.onClick.AddListener(() => OnEquipClicked?.Invoke());
        //closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
    }

    public void Show(SpecialNodeData node, SpellData spell)
    {
        nameText.text = node.nodeName;
        descText.text = node.description;
        extraText.text = $"추가 MP  Number |  {node.GetRequirementText()}";
        equipButton.interactable = node.CanApplyTo(spell);
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}