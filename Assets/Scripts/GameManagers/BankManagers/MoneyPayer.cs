using TMPro;
using UnityEngine;

// панель для давання коштів в банк
public class MoneyPayer : MonoBehaviour
{
    public Player Player;
    public GameObject MoneyPayerPanel;
    public GameObject PlayerMoneyPanel;
    public GameObject BankMoneyPanel;
    public GameObject ConditionsWarningPanel;
    public GameObject MoneyPayerObject;
    
    public int RequiredMoney;

    public void ShowMoney()
    {
        ShowBanknotes();
        ShowMoneySum();
        UpdateGivenMoneySum();
    }

    private void ShowBanknotes()
    {
        var banknoteNames = new[]
            { "Banknote5M", "Banknote2M", "Banknote1M", "Banknote500K", "Banknote200K", "Banknote100K" };
        byte i = 0;
        foreach (var banknote in Player.Money)
        {
            var banknoteRow = GameObject.Find($"PlayerMoneyPanel/{banknoteNames[i++]}");
            var banknoteText = banknoteRow.transform.Find("Count").GetComponent<TextMeshProUGUI>();
            banknoteText.text = $"x{banknote.Amount}";
        }
    }

    public void MoneyPayerPanelClose()
    {
        MoneyPayerPanel.SetActive(false);
    }

    public void ConditionsWarningPanelClose()
    {
        ConditionsWarningPanel.SetActive(false);
    }

    public void GiveBanknoteToBank(int banknoteIndex, out bool equalToZero)
    {
        var group = Player.Money[banknoteIndex];

        if (group.Amount <= 0)
        {
            equalToZero = false;
            return;
        }

        group.Amount--;
        RequiredMoney -= group.Banknote.Value;
        UpdateGivenMoneySum();
        ShowMoney();
        
        equalToZero = RequiredMoney == 0;
    }

    public void TakeBanknoteFromBank(int banknoteIndex, out bool equalToZero)
    {
        var group = Player.Money[banknoteIndex];
        
        group.Amount++;
        RequiredMoney += group.Banknote.Value;
        UpdateGivenMoneySum();
        ShowMoney();
        equalToZero = RequiredMoney == 0;
    }

    private void UpdateGivenMoneySum()
    {
        var sum = BankMoneyPanel.transform.Find("Sum").GetComponent<TextMeshProUGUI>();
        sum.text = $"Потрібно: {RequiredMoney}";
    }

    private void ShowMoneySum()
    {
        var moneySumText = GameObject.Find("Sum").GetComponent<TextMeshProUGUI>();
        moneySumText.text = $"Сума: {Player.MoneySum.ToString()}";
    }

    public void OnMouseDown()
    {
        MoneyPayerPanel.SetActive(true);
        ShowMoney();
    }
}