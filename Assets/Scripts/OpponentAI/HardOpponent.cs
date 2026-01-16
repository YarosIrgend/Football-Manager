using System.Collections;
using System.Linq;
using UnityEngine;

public class HardOpponent : Opponent
{
    public HardOpponent()
    {
        mistakeChance = 0.2f;
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

    public override IEnumerator HandleTransfer(Player self)
    {
        Init();
        var club = self.Clubs.FirstOrDefault(c => c.IsPlayable);
        if (club == null)
            yield break;

        var money = self.MoneySum;
        if (money < 1_000_000)
            yield break;

        if (club.Footballer == null)
        {
            yield return BuyBestFootballer(self, club);
            yield break;
        }

        if (club.Trainer == null)
        {
            yield return BuyBestTrainer(self, club);
            yield break;
        }

        if (club.Manager == null && money >= TransferManager.Manager.Price)
        {
            Bank.TakeMoney(self, TransferManager.Manager.Price);
            club.Manager = TransferManager.Manager;
            MessagePanelController.Instance.Show($"{self.ColorString} придбав у команду {club.Name}" +
                                                 " менеджера");
            yield return new WaitForSeconds(1.5f);
        }
    }

    private IEnumerator BuyBestFootballer(Player self, Club club)
    {
        Footballer best;
        if (Random.value < mistakeChance)
        {
            var random = new System.Random();
            var num = random.Next(1, 6);
            best = TransferManager.Footballers[num];
        }
        else
        {
            best = TransferManager.Footballers
                .Where(f => f.Price <= self.MoneySum)
                .OrderByDescending(f => f.Points)
                .FirstOrDefault();
        }

        if (best == null)
            yield break;


        Bank.TakeMoney(self, best.Price);
        club.Footballer = best;
        MessagePanelController.Instance.Show($"{self.ColorString} придбав у команду {club.Name}" +
                                             $" {best.Points}-очкового футболіста");
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator BuyBestTrainer(Player self, Club club)
    {
        Trainer best;
        if (Random.value < mistakeChance)
        {
            var random = new System.Random();
            var num = random.Next(1, 4);
            best = TransferManager.Trainers[num];
        }
        else
        {
            best = TransferManager.Trainers
                .Where(t => t.Price <= self.MoneySum)
                .OrderByDescending(t => t.Points)
                .FirstOrDefault();
        }

        if (best == null)
            yield break;

        Bank.TakeMoney(self, best.Price);
        club.Trainer = best;
        MessagePanelController.Instance.Show($"{self.ColorString} придбав у команду {club.Name}" +
                                             $" {best.Points}-очкового тренера");
        yield return new WaitForSeconds(1.5f);
    }
}