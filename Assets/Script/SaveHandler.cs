using UnityEngine;

public class SaveHandler : Singleton<SaveHandler>
{
    private void Start()
    {
        LoadGameData();
    }

    // 게임 종료 시 자동 저장
    private void OnApplicationQuit()
    {
        SaveGameData();
    }

    // 앱이 백그라운드로 진입할 때
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGameData();
        }
    }


    // =========================================================
    // Save
    // =========================================================

    public void SaveGameData()
    {
        PlayerSaveData saveData = new PlayerSaveData();

        Player player = Player.Instance;
        InventoryManager inventoryManager = InventoryManager.Instance;

        if (player == null || inventoryManager == null)
            return;


        // 1. 스탯 저장
        saveData.level = player.Stats.Level;


        // 2. 장착 상태 저장
        // 장착 상태는 InventoryManager가 관리한다.
        saveData.equippedItems =
            inventoryManager.GetEquippedItemSaveData();


        // 3. 인벤토리 저장
        saveData.inventoryItemIDs =
            inventoryManager.GetInventoryItemIDs();


        // 4. 자원 저장
        saveData.currentHealth =
            player.ResourceSystem.CurrentHealth;
        saveData.currentMana =
            player.ResourceSystem.CurrentMana;

        saveData.currentStamina =
            player.ResourceSystem.CurrentStamina;


        //Debug.Log($"inventory count: {saveData.inventoryItemIDs.Count}");
        //Debug.Log($"equipment count: {saveData.equippedItems.Count}");

        SpellProgressManager.Instance.Save();
        SaveDataManager.SaveData(saveData);

        //Debug.Log("===== SaveDataManager.SaveData =====");
    }


    // =========================================================
    // Load
    // =========================================================

    public void LoadGameData()
    {
        PlayerSaveData saveData = SaveDataManager.LoadData();

        Player player = Player.Instance;
        InventoryManager inventoryManager = InventoryManager.Instance;


        // 저장 데이터가 없다면 새 게임
        if (saveData == null)
        {
            if (inventoryManager != null)
            {
                inventoryManager.NewGame();
            }

            return;
        }


        if (player == null || inventoryManager == null)
            return;


        // =====================================================
        // 1. 레벨 및 스탯 복원
        // =====================================================

        if (player.Stats != null)
        {
            player.Stats.SetStats(saveData.level);
        }


        // =====================================================
        // 2. 자원 복원
        // =====================================================

        if (player.ResourceSystem != null)
        {
            player.ResourceSystem.SetResources(
                saveData.currentHealth,
                saveData.currentMana,
                saveData.currentStamina
            );
        }


        // =====================================================
        // 3. 인벤토리 복원
        // =====================================================

        if (saveData.inventoryItemIDs != null)
        {
            //Debug.Log("LoadInventory");

            inventoryManager.LoadInventory(saveData);
        }


        // =====================================================
        // 4. 장착 상태 복원
        // =====================================================

        if (saveData.equippedItems != null)
        {
            //Debug.Log("LoadEquipment");

            inventoryManager.LoadEquippedItems(
                saveData.equippedItems
            );
        }
    }
}
