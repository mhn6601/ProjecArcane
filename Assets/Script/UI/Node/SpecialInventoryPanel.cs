using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 보유 스페셜 노드 창 (인벤토리 형태)
public class SpecialInventoryPanel : MonoBehaviour
{
    [SerializeField] private RectTransform listRoot;
   // [SerializeField] private Button closeButton;    

    public event Action OnCloseClicked;

    private void Awake() { }// => closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

    public void Show(Vector3 slotWorldPos, SpellData spell, IEnumerable<SpecialNodeData> nodes)
    {
        for (int i = listRoot.childCount - 1; i >= 0; i--)
        {
            Transform child = listRoot.GetChild(i);
            if (child.GetComponent<SpecialInventoryEntry>() != null)   // 엔트리 컴포넌트 이름에 맞게 수정
            {
                child.SetParent(null);
                Destroy(child.gameObject);
            }
        }

        //closeButton.transform.position = slotWorldPos;   // 닫기 버튼을 슬롯 자리에 배치
        gameObject.SetActive(true);
    }

    public void Hide() => gameObject.SetActive(false);
}