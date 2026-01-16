using System.Globalization;
using TMPro;
using UnityEngine;

public class MoneyExchanger : MonoBehaviour
{
    public Player Player;
    public GameObject MoneyExchangerPanel;
    public GameObject PlayerMoneyPanel;
    public GameObject BankMoneyPanel;
    public GameObject ExchangeWarningPanel;
    
    public int GivenMoney;
    
    public void ShowMoney()
    {
        ShowBanknotes();
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
    
    public void MoneyExchangerPanelClose()
    {
        if (GivenMoney == 0)
        {
            MoneyExchangerPanel.SetActive(false);
        }
        else
        {
            ExchangeWarningPanel.SetActive(true);
        }
    }

    public void ExchangerWarningPanelClose()
    {
        ExchangeWarningPanel.SetActive(false);
    }
    
    public void GiveBanknoteToBank(int banknoteIndex)
    {
        var group = Player.Money[banknoteIndex];

        if (group.Amount <= 0) 
            return;

        group.Amount--;
        GivenMoney += group.Banknote.Value;
        UpdateGivenMoneySum();
        ShowMoney();
    }

    public void TakeBanknoteFromBank(int banknoteIndex)
    {
        var group = Player.Money[banknoteIndex];

        if (GivenMoney < group.Banknote.Value) 
            return;

        group.Amount++;
        GivenMoney -= group.Banknote.Value;
        UpdateGivenMoneySum();
        ShowMoney();
    }

    private void UpdateGivenMoneySum()
    {
        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        nfi.NumberGroupSeparator = " ";
        var sum = BankMoneyPanel.transform.Find("Sum").GetComponent<TextMeshProUGUI>();
        sum.text = $"Дано: {GivenMoney.ToString("N", nfi)}";
    }
    
    public void OnMouseDown()
    {
        MoneyExchangerPanel.SetActive(true);
        ShowMoney();
    }
}