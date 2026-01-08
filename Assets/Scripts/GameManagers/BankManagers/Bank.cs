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
        while (money > 0)
        {
            var banknote = Banknotes.First(b => b.Value <= money);
            var playerGroup = player.Money
                .First(g => g.Banknote.Value == banknote.Value);

            playerGroup.Amount++;
            money -= banknote.Value;
        }
    }

    // Автоматичне зняття грошей (для ботів, податків і т.п.)
    public void TakeMoney(Player player, int requiredMoney)
    {
        int totalPaid = GetMinimalPayableSum(player, requiredMoney);

        if (totalPaid < requiredMoney)
        {
            Debug.LogError("Player does not have enough money");
            return;
        }

        RemoveMoney(player, totalPaid);

        int change = totalPaid - requiredMoney;
        if (change > 0)
            AddMoney(player, change);
        
    }

    private void RemoveMoney(Player player, int amount)
    {
        foreach (var group in player.Money.OrderByDescending(b => b.Banknote.Value))
        {
            while (group.Amount > 0 && amount >= group.Banknote.Value)
            {
                group.Amount--;
                amount -= group.Banknote.Value;
            }

            if (amount == 0)
                return;
        }
    }

    private int GetMinimalPayableSum(Player player, int required)
    {
        int sum = 0;

        foreach (var group in player.Money.OrderByDescending(b => b.Banknote.Value))
        {
            sum += group.Banknote.Value * group.Amount;
            if (sum >= required)
                return sum;
        }

        return sum;
    }
}