using System.Globalization;
using TMPro;
using UnityEngine;

public class MoneyPanelController : MonoBehaviour
{
    public GameObject MoneyPanel;

    public void ShowMoney(Player player)
    {
        ShowBanknotes(player);
        ShowMoneySum(player);
    }

    private static void ShowBanknotes(Player player)
    {
        var banknoteNames = new[]
            { "Banknote5M", "Banknote2M", "Banknote1M", "Banknote500K", "Banknote200K", "Banknote100K" };
        byte i = 0;
        foreach (var banknote in player.Money)
        {
            var banknoteRow = GameObject.Find(banknoteNames[i++]);
            var banknoteText = banknoteRow.transform.Find("Count").GetComponent<TextMeshProUGUI>();
            banknoteText.text = $"x{banknote.Amount}";
        }
    }

    private static void ShowMoneySum(Player player)
    {
        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        nfi.NumberGroupSeparator = " ";
        var moneySumText = GameObject.Find("Sum").GetComponent<TextMeshProUGUI>();
        moneySumText.text = $"Сума: {player.MoneySum.ToString("N", nfi)}";
    }
}