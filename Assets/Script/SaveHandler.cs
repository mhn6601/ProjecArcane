using Unity.VisualScripting;
using UnityEngine;

public class SaveHandler : Singleton<SaveHandler>
{
    Player player;

    private void Start()
    {
        LoadGameData();
    }

    // 게임 종료 시 자동 저장 (PC / 콘솔)
    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    // 앱이 배경으로 넘어가거나 백그라운드로 진입할 때 (모바일/에디터 focus)
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameData();
        }
    }

    public void SaveGameData()
    {
        //Debug.Log("SaveGameData");
        PlayerSaveData saveData = new PlayerSaveData();
        player = Player.Instance;
        // 1. 스탯 및 자원 수치 저장
        saveData.level = player.Stats.Level;
        saveData.equippedItems = player.EquipmentManager.GetEquippedItemSaveData();
        saveData.currentHealth = player.ResourceSystem.CurrentHealth;
        saveData.currentStamina = player.ResourceSystem.CurrentStamina;


        SaveDataManager.SaveData(saveData);
    }

    public void LoadGameData()
    {
        PlayerSaveData saveData = SaveDataManager.LoadData();
        Player player = Player.Instance;

        if (player == null) return;

        // 1. 레벨 및 스탯 적용
        if (player.Stats != null)
        {
            player.Stats.SetStats(saveData.level);
        }

        // 2. 체력/마나/스태미나 자원 적용
        if (player.ResourceSystem != null)
        {
            player.ResourceSystem.SetResources(saveData.currentHealth, saveData.currentMana, saveData.currentStamina);
        }

        // 3. 장착 장비 데이터 복원 (저장된 딕셔너리와 DB 에셋 전달)
        if (player.EquipmentManager != null && saveData.equippedItems != null)
        {
            player.EquipmentManager.LoadEquippedItems(saveData.equippedItems, InventoryManager.Instance.GetEquipmentDB());
        }
    }
}