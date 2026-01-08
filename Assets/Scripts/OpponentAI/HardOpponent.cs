using System.Linq;

public class HardOpponent : Opponent
{
    public HardOpponent()
    {
        mistakeChance = 0.1f;
    }

    public override bool DecideBuyProperty(Game game, Player self, Property property)
    {
        Init();

        bool smartDecision = DecisionService.ShouldBuy(game, self, property);

        if (RollMistake())
            return !smartDecision;

        return smartDecision;
    }


    public override bool TryResolveMoney(Player self, int requiredMoney)
    {
        Init();
        return DecisionService.TryResolveMoney(self, requiredMoney);
    }


    public override void HandleTransfer(Player self)
    {
        Init();

        var club = self.Clubs.FirstOrDefault(c => c.IsPlayable);
        if (club == null)
            return;

        int money = MoneyUtils.GetTotalMoney(self);
        if (money < 1_000_000)
            return;

        if (club.Footballer == null)
        {
            BuyBestFootballer(self, club);
            return;
        }

        if (club.Trainer == null)
        {
            BuyBestTrainer(self, club);
            return;
        }

        if (club.Manager == null && money >= TransferManager.Manager.Price)
        {
            Bank.TakeMoney(self, TransferManager.Manager.Price);
            club.Manager = TransferManager.Manager;
        }
    }

    private void BuyBestFootballer(Player self, Club club)
    {
        var best = TransferManager.Footballers
            .Where(f => f.Price <= MoneyUtils.GetTotalMoney(self))
            .OrderByDescending(f => f.Points)
            .FirstOrDefault();

        if (best == null)
            return;

        Bank.TakeMoney(self, best.Price);
        club.Footballer = best;
    }

    private void BuyBestTrainer(Player self, Club club)
    {
        var best = TransferManager.Trainers
            .Where(t => t.Price <= MoneyUtils.GetTotalMoney(self))
            .OrderByDescending(t => t.Points)
            .FirstOrDefault();

        if (best == null)
            return;

        Bank.TakeMoney(self, best.Price);
        club.Trainer = best;
    }
}
