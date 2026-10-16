using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialNodeDatabase", menuName = "Database/SpecialNode Database")]
public class SpecialNodeDatabaseSO : ScriptableObject
{
    // 프로젝트 내에 존재하는 모든 ItemSO 에셋을 등록해 두는 리스트
    [SerializeField] private List<SpecialNodeData> allSpecialNodes = new List<SpecialNodeData>();

    /// <summary>
    /// 저장 파일에서 읽어온 itemID로 일치하는 ItemSO 객체를 검색
    /// </summary>

    public SpecialNodeData GetSpecialNodeByID(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        // allItems 중 EquipmentItemData 타입인 것만 필터링 후 ID 조건 비교
        return allSpecialNodes.FirstOrDefault(item => item != null && item.nodeId == id);
    }
    public List<SpecialNodeData> GetNodeList()
    {
        return allSpecialNodes;
    }
}