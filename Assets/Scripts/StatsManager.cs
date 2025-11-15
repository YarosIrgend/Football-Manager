using System.IO;
using UnityEngine;

[System.Serializable]
public class StatsData
{
    public int matches;
    public int wins;
    public int loses;
    public int income;
    public int expenses;
    public int maxBudget;
}


public class StatsManager : MonoBehaviour
{
    private string filePath;
    public StatsData stats;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "stats.json");

        LoadStats();
    }

    public void LoadStats()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            stats = JsonUtility.FromJson<StatsData>(json);
            Debug.Log("Статистика завантажена");
        }
        else
        {
            stats = new StatsData(); 
            SaveStats();
            Debug.Log("Створено нову статистику");
        }
    }

    public void SaveStats()
    {
        string json = JsonUtility.ToJson(stats, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Статистика збережена");
    }

    public void AddMatch(bool win)
    {
        stats.matches++;

        if (win) stats.wins++;
        else stats.loses++;

        SaveStats();
    }
}