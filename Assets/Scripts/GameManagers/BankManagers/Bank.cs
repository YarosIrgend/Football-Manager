// для обміну купюр

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Bank : MonoBehaviour
{
    public GameObject MoneyExchangerPanel;
    public MoneyExchanger MoneyExchanger;
    
    private List<Banknote> Banknotes = new()
    {
        new Banknote { Value = 5_000_000 },
        new Banknote { Value = 2_000_000 },
        new Banknote { Value = 1_000_000 },
        new Banknote { Value = 500_000 },
        new Banknote { Value = 200_000 },
        new Banknote { Value = 100_000 }
    };

    public void AddMoney(Player player, int money)
    {
        while (money != 0)
        {
            var banknoteToPay = Banknotes.First(banknote => banknote.Value <= money);
            var playerBanknoteToAdd =
                player.Money.Find(banknoteGroup => banknoteGroup.Banknote.Value == banknoteToPay.Value);
            playerBanknoteToAdd.Amount++;
            money -= banknoteToPay.Value;
        }
    }

    public void OnMouseDown()
    {
        MoneyExchangerPanel.SetActive(true);
        MoneyExchanger.ShowMoney();
    }
}