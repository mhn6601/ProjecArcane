using System.IO;
using UnityEngine;

public static class SpellSaveSystem
{
    private static string Path =>
        System.IO.Path.Combine(Application.persistentDataPath, "spell_save.json");

    public static void Save(SpellSaveData data)
    {
        try
        {
            File.WriteAllText(Path, JsonUtility.ToJson(data, true));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"스펠 저장 실패: {e.Message}");
        }
    }

    public static SpellSaveData Load()
    {
        try
        {
            if (File.Exists(Path))
            {
                var data = JsonUtility.FromJson<SpellSaveData>(File.ReadAllText(Path));
                if (data != null) return data;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"스펠 로드 실패: {e.Message}");
        }
        return new SpellSaveData();
    }
}