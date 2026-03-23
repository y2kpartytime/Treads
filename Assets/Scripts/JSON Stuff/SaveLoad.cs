using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    string path;
    public GamePlayData currentGameData;
    public StatisticalData currentStats;
    public EnvironmentData currentEnv;

    void Awake()
    {
        path = Application.persistentDataPath + "/save.json";
        LoadGame();
    }

    public void SaveGame(GamePlayData gameData, StatisticalData stats, EnvironmentData env)
    {
        SaveWrapper wrapper = new SaveWrapper
        {
            gamePlayData = gameData,
            statisticalData = stats,
            environmentData = env
        };

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);

        Debug.Log("Game Saved!");
    }

    public SaveWrapper LoadGame()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveWrapper data = JsonUtility.FromJson<SaveWrapper>(json);
            Debug.Log("Game Loaded!");
            return data;
        }

        return new SaveWrapper();
    }

    void OnApplicationQuit()
    {
        SaveGame(currentGameData, currentStats, currentEnv);
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame(currentGameData, currentStats, currentEnv);
        }
    }
}

[System.Serializable]
public class SaveWrapper
{
    public GamePlayData gamePlayData;
    public StatisticalData statisticalData;
    public EnvironmentData environmentData;
}