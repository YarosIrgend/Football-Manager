using TMPro;
using UnityEngine;

public class StatsPanelController : MonoBehaviour
{
    public StatsManager statsManager;

    [Header("Value Fields")]
    public TMP_Text easyWinsValue;
    public TMP_Text hardWinsValue;
    public TMP_Text lossesValue;
    public TMP_Text incomeValue;
    public TMP_Text expensesValue;
    public TMP_Text maxBudgetValue;

    private void OnEnable()
    {
        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        easyWinsValue.text = statsManager.stats.winsOnEasy.ToString();
        hardWinsValue.text = statsManager.stats.winsOnHard.ToString();
        lossesValue.text = statsManager.stats.losses.ToString();
        incomeValue.text = statsManager.stats.income.ToString();
        expensesValue.text = statsManager.stats.expenses.ToString();
        maxBudgetValue.text = statsManager.stats.maxBudget.ToString();
    }
    
    public void ClearStats()
    {
        Debug.Log("Очищається");
        statsManager.stats.winsOnEasy = 0;
        statsManager.stats.winsOnHard = 0;
        statsManager.stats.losses = 0;
        statsManager.stats.income = 0;
        statsManager.stats.expenses = 0;
        statsManager.stats.maxBudget = 0;
        UpdateStatsUI();
        statsManager.SaveStats();
    }
}

