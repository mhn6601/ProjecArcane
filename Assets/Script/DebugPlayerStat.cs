using TMPro;
using UnityEngine;

public class DebugPlayerStat : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI TextMeshProUGUI;

    PlayerStats playerStats;


    private void Start()
    {
        playerStats = Player.Instance.Stats;
        playerStats.OnStatsChanged += UpdateText;
    }
    void UpdateText()
    {
        TextMeshProUGUI.text=$"SpellPower:{playerStats.SpellPower}";
    }
}
