using UnityEngine;
using System.IO;
using System.Linq;

public static class SaveManager
{
    static string SavePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    static SaveData saveData;

    public static void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            saveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            saveData = new SaveData();
        }
    }

    public static void Save()
    {
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
    }

    public static void CompleteLevel(string sceneName, float time)
    {
        var level = saveData.levels
            .FirstOrDefault(x => x.sceneName == sceneName);

        if (level == null)
        {
            level = new LevelSaveData
            {
                sceneName = sceneName,
                completed = true,
                bestTime = time
            };

            saveData.levels.Add(level);
        }
        else
        {
            level.completed = true;

            if (level.bestTime <= 0 || time < level.bestTime)
                level.bestTime = time;
        }

        Save();
    }

    public static LevelSaveData GetLevelData(string sceneName)
    {
        return saveData.levels
            .FirstOrDefault(x => x.sceneName == sceneName);
    }
}