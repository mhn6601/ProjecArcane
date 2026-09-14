using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentItemDatabase", menuName = "Data/EquipmentItem Database")]
public class EquipmentItemDatabaseSO : ScriptableObject
{
    // 프로젝트 내에 존재하는 모든 ItemSO 에셋을 등록해 두는 리스트
    [SerializeField] private List<EquipmentItemData> allItems = new List<EquipmentItemData>();

    /// <summary>
    /// 저장 파일에서 읽어온 itemID로 일치하는 ItemSO 객체를 검색
    /// </summary>
   
    public EquipmentItemData GetItemByID(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        // allItems 중 EquipmentItemData 타입인 것만 필터링 후 ID 조건 비교
        return allItems.FirstOrDefault(item => item != null && item.itemId == id);
    }
}