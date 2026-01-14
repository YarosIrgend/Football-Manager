using System.IO;
using UnityEngine;

[System.Serializable]
public class StatsData
{
    public ushort winsOnEasy;
    public ushort winsOnHard;
    public ushort losses;
    public ulong income;
    public ulong expenses;
    public uint maxBudget;
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

    public ulong GetStat(string key)
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

    public void AddToStat(string key, ulong amount = 1)
    {
        switch (key)
        {
            case "winsOnEasy": stats.winsOnEasy += (ushort)amount; break;
            case "winsOnHard": stats.winsOnHard += (ushort)amount; break;
            case "losses": stats.losses += (ushort)amount; break;
            case "income": stats.income += amount; break;
            case "expenses": stats.expenses += amount; break;
            case "maxBudget": stats.maxBudget = (uint)amount; break;
        }

        SaveStats();
    }
}