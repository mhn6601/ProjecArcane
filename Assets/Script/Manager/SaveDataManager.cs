using System.IO;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
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
            Debug.Log("[SaveManager] 저장 파일이 존재하지 않습니다. 기본 데이터 세팅 및 저장 파일을 생성합니다.");

            // 1. 기본 데이터 객체 생성
            PlayerSaveData defaultData = new PlayerSaveData();

            // 2. 기본 데이터를 디스크 파일(JSON)로 저장
            SaveData(defaultData);

            return defaultData;
        }

        string json = File.ReadAllText(SavePath);
        PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);
        Debug.Log("[SaveManager] Data Loaded Successfully.");
        return data;
    }
}