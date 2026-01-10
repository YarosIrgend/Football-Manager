using System.Linq;

public class EasyOpponent : Opponent
{
    public EasyOpponent()
    {
        mistakeChance = 0.4f;
    }

    public override bool DecideBuyProperty(Game game, Player self, Property property)
    {
        if (RollMistake())
            return UnityEngine.Random.value > 0.5f;

        return MoneyUtils.GetTotalMoney(self) >= property.Price;
    }

    public override bool TryResolveMoney(Player self, int requiredMoney)
    {
        return MoneyUtils.GetTotalMoney(self) >= requiredMoney;
    }

    public override void HandleTransfer(Player self)
    {
        var club = self.Clubs.FirstOrDefault(c => c.IsPlayable);
        if (club == null)
            return;

        if (club.Footballer == null)
        {
            TryBuyFootballer(self, club);
            return;
        }

        if (club.Trainer == null)
        {
            TryBuyTrainer(self, club);
            return;
        }

        if (club.Manager == null && UnityEngine.Random.value > 0.5f)
        {
            TryBuyManager(self, club);
        }
    }

    private void TryBuyFootballer(Player self, Club club)
    {
        var f = TransferManager.Footballers.GetRandomItem();
        if (MoneyUtils.GetTotalMoney(self) < f.Price)
            return;

        Bank.TakeMoney(self, f.Price);
        club.Footballer = f;
    }

    private void TryBuyTrainer(Player self, Club club)
    {
        var t = TransferManager.Trainers.GetRandomItem();
        if (MoneyUtils.GetTotalMoney(self) < t.Price)
            return;

        Bank.TakeMoney(self, t.Price);
        club.Trainer = t;
    }

    private void TryBuyManager(Player self, Club club)
    {
        var m = TransferManager.Manager;
        if (MoneyUtils.GetTotalMoney(self) < m.Price)
            return;

        Bank.TakeMoney(self, m.Price);
        club.Manager = m;
    }
}
