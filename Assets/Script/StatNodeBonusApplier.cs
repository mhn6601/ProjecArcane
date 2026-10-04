using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class StatNodeBonusApplier : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private List<StatNodeData> allStatNodes = new List<StatNodeData>();

    private Dictionary<string, StatNodeData> map;

    private void Awake()
    {
        map = new Dictionary<string, StatNodeData>();
        foreach (var node in allStatNodes)
        {
            if (node != null) map[node.nodeId] = node;
        }
    }

    private void Start()
    {
        var progress = SpellProgressManager.Instance;

        // 로드된 세이브 기준으로 이미 해금된 노드 적용
        foreach (var node in allStatNodes)
        {
            if (node != null && progress.IsUnlocked(node.nodeId, SpellNodeType.Stat))
                Apply(node);
        }

        progress.OnSpellUnlocked += HandleUnlocked;
    }

    private void OnDestroy()
    {
        if (SpellProgressManager.Instance != null)
            SpellProgressManager.Instance.OnSpellUnlocked -= HandleUnlocked;
    }

    private void HandleUnlocked(string id)
    {
        if (map.TryGetValue(id, out var node)) Apply(node);
    }

    private void Apply(StatNodeData node)
    {
        playerStats.SetBonuses(node.nodeId, node.bonuses);
    }
}