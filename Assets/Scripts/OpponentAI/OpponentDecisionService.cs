using System.Collections.Generic;
using System.Linq;

public class OpponentDecisionService
{
    private readonly Bank bank;

    public OpponentDecisionService(Bank bank)
    {
        this.bank = bank;
    }

    public bool ShouldBuy(Game game, Player self, Property property)
    {
        GameState state = GameState.FromGame(game, self);

        SimulatedAction buyAction = SimulatedAction.Buy(property);
        float score = MinimaxSolver.Evaluate(state, buyAction, depth: 2);

        return score > 0;
    }

    public bool TryResolveMoney(Player self, int requiredMoney)
    {
        if (self.MoneySum >= requiredMoney)
            return true;

        // 1️⃣ пробуємо заставу
        foreach (var p in GetAllProperties(self))
        {
            if (!p.IsMortgaged)
            {
                p.IsMortgaged = true;
                bank.AddMoney(self, p.Price / 2);

                if (self.MoneySum >= requiredMoney)
                    return true;
            }
        }

        // 2️⃣ продаємо найдешевше
        var sellable = GetAllProperties(self)
            .OrderBy(p => p.Price)
            .FirstOrDefault();

        if (sellable != null)
        {
            RemoveProperty(self, sellable);
            bank.AddMoney(self, sellable.Price);

            return self.MoneySum >= requiredMoney;
        }

        return false;
    }

    private IEnumerable<Property> GetAllProperties(Player player)
    {
        foreach (var c in player.Clubs) yield return c;
        foreach (var t in player.Telecompanies) yield return t;
    }

    private void RemoveProperty(Player player, Property property)
    {
        if (property is Club c)
            player.Clubs.Remove(c);
        else if (property is Telecompany t)
            player.Telecompanies.Remove(t);
    }
}