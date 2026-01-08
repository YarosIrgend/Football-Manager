using System.Linq;

public static class MoneyUtils
{
    public static int GetTotalMoney(Player player)
    {
        return player.Money.Sum(g => g.Banknote.Value * g.Amount);
    }
}