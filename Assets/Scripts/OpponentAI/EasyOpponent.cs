using System.Collections;
using System.Linq;
using UnityEngine;

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

    public override IEnumerator HandleTransfer(Player self)
    {
        Init();
        var club = self.Clubs.FirstOrDefault(c => c.IsPlayable);
        if (club == null)
            yield break;

        if (club.Footballer == null)
        {
            yield return TryBuyFootballer(self, club);
            yield break;
        }

        if (club.Trainer == null)
        {
            yield return TryBuyTrainer(self, club);
            yield break;
        }

        if (club.Manager == null && UnityEngine.Random.value > 0.5f)
        {
            yield return TryBuyManager(self, club);
        }
    }

    private IEnumerator TryBuyFootballer(Player self, Club club)
    {
        var f = TransferManager.Footballers.GetRandomItem();
        if (MoneyUtils.GetTotalMoney(self) < f.Price)
            yield break;

        Bank.TakeMoney(self, f.Price);
        club.Footballer = f;
        MessagePanelController.Instance.Show($"{self.ColorString} придбав у команду {club.Name}" +
                                             $" {f.Points}-очкового футболіста");
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator TryBuyTrainer(Player self, Club club)
    {
        var t = TransferManager.Trainers.GetRandomItem();
        if (MoneyUtils.GetTotalMoney(self) < t.Price)
            yield break;

        Bank.TakeMoney(self, t.Price);
        club.Trainer = t;
        MessagePanelController.Instance.Show($"{self.ColorString} придбав у команду {club.Name}" +
                                             $" {t.Points}-очкового тренера");
        yield return new WaitForSeconds(1.5f);
    }

    private IEnumerator TryBuyManager(Player self, Club club)
    {
        var m = TransferManager.Manager;
        if (MoneyUtils.GetTotalMoney(self) < m.Price)
            yield break;

        Bank.TakeMoney(self, m.Price);
        club.Manager = m;
        MessagePanelController.Instance.Show($"{self.ColorString} придбав у команду {club.Name}" +
                                             " менеджера");
        yield return new WaitForSeconds(1.5f);
    }
}
