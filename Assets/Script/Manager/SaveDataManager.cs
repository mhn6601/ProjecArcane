using System.IO;
using UnityEngine;

public class SaveDataManager : Singleton<SaveDataManager>
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "player_save.json");

    // 데이터 저장
    public static void SaveData(PlayerSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"[SaveManager] Data Saved to: {SavePath}");
    }

    // 데이터 불러오기
    public static PlayerSaveData LoadData()
    {
        if (!File.Exists(SavePath))
        {
            //Debug.Log("[SaveManager] ==null ");
            return null ;
        }

        string json = File.ReadAllText(SavePath);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
        //Debug.Log("[SaveManager] Data Loaded Successfully.");
        return data;
    }
}