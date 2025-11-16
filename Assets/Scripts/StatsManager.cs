using System.IO;
using UnityEngine;

[System.Serializable]
public class StatsData
{
    public int winsOnEasy;
    public int winsOnHard;
    public int losses;
    public int income;
    public int expenses;
    public int maxBudget;
}


public class StatsManager : MonoBehaviour
{
    public StatsData stats;
    private string filePath;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "stats.json");
        LoadStats();
    }

    public void LoadStats()
    {
        if (!File.Exists(filePath))
        {
            stats = new StatsData();
            SaveStats();
            return;
        }

        string json = File.ReadAllText(filePath);
        stats = JsonUtility.FromJson<StatsData>(json);
    }

    public void SaveStats()
    {
        string json = JsonUtility.ToJson(stats, true);
        File.WriteAllText(filePath, json);
    }

    public int GetStat(string key)
    {
        return key switch
        {
            "winsOnEasy" => stats.winsOnEasy,
            "winsOnHard" => stats.winsOnHard,
            "losses" => stats.losses,
            "income" => stats.income,
            "expenses" => stats.expenses,
            "maxBudget" => stats.maxBudget,
            _ => 0
        };
    }

    public void AddToStat(string key, int amount)
    {
        switch (key)
        {
            case "winsOnEasy": stats.winsOnEasy += amount; break;
            case "winsOnHard": stats.winsOnHard += amount; break;
            case "losses": stats.losses += amount; break;
            case "income": stats.income += amount; break;
            case "expenses": stats.expenses += amount; break;
            case "maxBudget": stats.maxBudget += amount; break;
        }

        SaveStats();
    }
}