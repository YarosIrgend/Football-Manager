using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class CellActionManager : MonoBehaviour
{
    public MoneyPayer MoneyPayer;
    public GameManager GameManager;
    public CellManager CellManager;
    public Bank Bank;

    public GameObject BuyingChoice; // в панелі PropertyInfoPanel

    private bool BuyChoice;

    [HideInInspector] public Property PendingPurchase; // нерухомість, яку гравець вирішив купити

    // спільний для гравця та противників
    // у Handles окремі обробки для гравця та противників
    public IEnumerator DoActionAccordingCellCoroutine(Game game, Cell cell, Player player, Action<bool> onCompleted)
    {
        bool completed = false;

        switch (cell.Type)
        {
            case CellType.Start:
                HandleStart(player);
                completed = true;
                break;

            case CellType.Club:
                yield return HandleClubCoroutine(game, cell, player, r => completed = r);
                break;

            case CellType.Telecompany:
                yield return HandleTelecompanyCoroutine(game, cell, player, r => completed = r);
                break;

            case CellType.Bonus:
                yield return HandleBonusCoroutine(player, game);
                completed = true;
                break;

            case CellType.Fine:
                yield return HandleFineCoroutine(game, player, r => completed = r);
                break;

            case CellType.Tax:
                yield return HandleTaxCoroutine(player, r => completed = r);
                break;

            case CellType.Transfer:
                yield return HandleTransferCoroutine(game, player, r => completed = r);
                SetTransferButtonActive(false);
                break;

            case CellType.Disqualification:
                yield return HandleDisqualificationCoroutine(player);
                completed = true;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        onCompleted?.Invoke(completed);
    }

    private void HandleStart(Player player)
    {
        Bank.AddMoney(player, 500_000);
    }

    private IEnumerator HandleClubCoroutine(Game game, Cell cell, Player player, Action<bool> onCompleted)
    {
        MessagePanelController.Instance.Show($"Клуб - {cell.CellName}");
        yield return new WaitForSeconds(1.5f);

        var club =
            GameManager.Game.Clubs.FirstOrDefault(t => t.Name == cell.CellName);

        // визначити, чи хтось володіє клубом, якщо так треба матч провести (якщо клуби є)
        var owner = game.Players.FirstOrDefault(p => p.Clubs.Any(tc => tc.Name == club!.Name));
        if (owner != null)
        {
            if (owner == player)
            {
                yield break;
            }

            MessagePanelController.Instance.Show(
                $"Є гравець {owner.ColorString}, який володіє цим клубом.");
            yield return new WaitForSeconds(1.5f);

            var ownerClub = owner.Clubs.FirstOrDefault(t => t.Name == cell.CellName);
            // якщо клуб є, треба провести матч за можливості
            yield return StartCoroutine(HoldTheMatchCouroutine(owner, player, ownerClub));
            onCompleted(MoneyPayer.RequiredMoney == 0);
        }

        // якщо ніхто не володіє, то можна придбати за бажанням
        else
        {
            if (player.Opponent == null)
            {
                ShowPropertyWithChoiceToBuy(cell, player);
                // Чекаємо натискання кнопки
                while (CellManager.BuyingChoice.activeSelf)
                    yield return null;

                if (BuyChoice)
                {
                    MoneyPayer.SetPayment(club.Price);
                    club.Footballer = null;
                    club.Trainer = null;
                    club.Manager = null;
                    onCompleted(false);
                    GameManager.MatchStatsData.clubsBought++;
                    GameManager.StatsManager.AddToStat("expenses", (ulong)club.Price);
                }
                else
                {
                    onCompleted(true);
                }
            }
            else
            {
                // рішення купити (обробити, якщо вирішив купити, то додати клуб)
                bool wantBuy = player.Opponent.DecideBuyProperty(game, player, club);

                if (!wantBuy)
                {
                    onCompleted(true);
                    yield break;
                }

                // спроба знайти бабло
                bool canPay = player.Opponent.TryResolveMoney(player, club.Price);

                if (!canPay)
                {
                    onCompleted(true);
                    yield break;
                }

                Bank.TakeMoney(player, club.Price);
                club.Footballer = null;
                club.Trainer = null;
                club.Manager = null;
                player.Clubs.Add(club);

                MessagePanelController.Instance.Show($"Гравець {player.ColorString} купив клуб {club.Name}");
                yield return new WaitForSeconds(1.5f);
                onCompleted(true);
            }
        }
    }

    private IEnumerator HandleTelecompanyCoroutine(Game game, Cell cell, Player player, Action<bool> onCompleted)
    {
        MessagePanelController.Instance.Show($"Телекомпанія - {cell.CellName}");
        yield return new WaitForSeconds(1.5f);

        var telecompany =
            GameManager.Game.Telecompanies.FirstOrDefault(t => t.Name == cell.CellName);

        // визначити, чи хтось володіє телекомпанією, якщо так треба платити
        var owner = game.Players.FirstOrDefault(p => p.Telecompanies.Any(tc => tc.Name == telecompany!.Name));
        if (owner != null)
        {
            if (owner == player)
            {
                yield break;
            }

            MessagePanelController.Instance.Show(
                $"Є гравець {owner.ColorString}, який володіє цією компанією. Усього: {owner.Telecompanies.Count}");
            yield return new WaitForSeconds(1.5f);

            if (telecompany.IsMortgaged)
            {
                MessagePanelController.Instance.Show("Телекомпанія закладена");
                yield return new WaitForSeconds(1.5f);
                yield break;
            }

            var payment = owner.Telecompanies.Count switch
            {
                1 => 300_000,
                2 => 500_000,
                3 => 1_000_000,
                _ => 2_000_000
            };

            if (player.Opponent == null) // наш гравець платить
            {
                MoneyPayer.SetPayment(payment);
                Bank.AddMoney(owner, payment);
                onCompleted(false);
                GameManager.StatsManager.AddToStat("expenses", (ulong)payment);
            }
            else // противник платить
            {
                bool canPay = player.Opponent.TryResolveMoney(player, payment);

                if (!canPay)
                {
                    Bank.AddMoney(owner, (int)player.MoneySum); // оплата, що лишилося у гравця, який платить
                    yield return DeclareBankruptcy(player);
                    onCompleted(true);
                    yield break;
                }

                Bank.TakeMoney(player, payment);
                Bank.AddMoney(owner, payment);

                if (owner.Opponent == null)
                {
                    GameManager.StatsManager.AddToStat("income", (ulong)payment);
                    if ((uint)GameManager.StatsManager.GetStat("maxBudget") <= owner.MoneySum)
                        GameManager.StatsManager.AddToStat("maxBudget", owner.MoneySum);
                }
            }
        }

        // якщо ніхто не володіє, то можна придбати за бажанням
        else
        {
            if (player.Opponent == null)
            {
                ShowPropertyWithChoiceToBuy(cell, player);
                // Чекаємо натискання кнопки
                while (CellManager.BuyingChoice.activeSelf)
                    yield return null;

                if (BuyChoice)
                {
                    MoneyPayer.SetPayment(telecompany.Price);
                    onCompleted(false);
                    GameManager.MatchStatsData.telecompaniesBought++;
                    GameManager.StatsManager.AddToStat("expenses", (ulong)telecompany.Price);
                }
                else
                {
                    onCompleted(true);
                }
            }
            else
            {
                bool wantBuy = player.Opponent.DecideBuyProperty(game, player, telecompany);

                if (!wantBuy)
                {
                    onCompleted(true);
                    yield break;
                }

                bool canPay = player.Opponent.TryResolveMoney(player, telecompany.Price);

                if (!canPay)
                {
                    onCompleted(true);
                    yield break;
                }

                Bank.TakeMoney(player, telecompany.Price);
                player.Telecompanies.Add(telecompany);
                MessagePanelController.Instance.Show(
                    $"Гравець {player.ColorString} купив телекомпанію {telecompany.Name}");
                yield return new WaitForSeconds(1.5f);
                onCompleted(true);
            }
        }
    }

    private IEnumerator HandleTransferCoroutine(Game game, Player player, Action<bool> onCompleted)
    {
        // рішення купити (обробити, якщо вирішив купити, то додати когось, кого він хоче)
        // але за правилами, що спочатку в клуб може купитися футболіст, потім тренер, потім менеджер за бажанням

        if (player.Opponent != null)
        {
            yield return StartCoroutine(player.Opponent.HandleTransfer(player));
            onCompleted(true);
            yield break;
        }
        
        SetTransferButtonActive();
        
        bool finished = false;

        TransferFlowController.Instance.StartTransfer(game, player, () => finished = true);

        while (!finished)
            yield return null;

        onCompleted(true);
    }

    private IEnumerator HandleBonusCoroutine(Player player, Game game)
    {
        var bonus = game.Bonuses.GetRandomItem();

        MessagePanelController.Instance.Show($"Бонус: {bonus.Value}");
        yield return new WaitForSeconds(1.5f);

        Bank.AddMoney(player, bonus.Value);
        if (player.Opponent == null)
        {
            GameManager.StatsManager.AddToStat("income", (ulong)bonus.Value);
            if (GameManager.StatsManager.GetStat("maxBudget") <= player.MoneySum)
                GameManager.StatsManager.AddToStat("maxBudget", player.MoneySum);
        }
    }

    private IEnumerator HandleFineCoroutine(Game game, Player player, Action<bool> onCompleted)
    {
        var fine = game.Fines.GetRandomItem();

        MessagePanelController.Instance.Show($"Штраф: {fine.Value}");
        yield return new WaitForSeconds(1.5f);

        if (player.Opponent == null) // наш гравець платить
        {
            MoneyPayer.SetPayment(fine.Value);
            onCompleted(false);
        }
        else // противник платить
        {
            bool canPay = player.Opponent.TryResolveMoney(player, fine.Value);

            if (!canPay)
            {
                yield return DeclareBankruptcy(player);
                onCompleted(true);
                yield break;
            }

            Bank.TakeMoney(player, fine.Value);
            GameManager.StatsManager.AddToStat("expenses", (ulong)fine.Value);
            onCompleted(true);
        }
    }

    private IEnumerator HandleTaxCoroutine(Player player, Action<bool> onCompleted)
    {
        int tax = 1_000_000;

        MessagePanelController.Instance.Show($"Податок: {tax}");
        yield return new WaitForSeconds(1.5f);

        if (player.Opponent == null)
        {
            MoneyPayer.SetPayment(tax);
            onCompleted(false);
        }
        else
        {
            bool canPay = player.Opponent.TryResolveMoney(player, tax);

            if (!canPay)
            {
                yield return DeclareBankruptcy(player);
                onCompleted(true);
                yield break;
            }

            Bank.TakeMoney(player, tax);
            onCompleted(true);
            GameManager.StatsManager.AddToStat("expenses", (ulong)tax);
        }
    }

    private IEnumerator HandleDisqualificationCoroutine(Player player)
    {
        MessagePanelController.Instance.Show($"Гравець {player.ColorString} отримує дискваліфікацію " +
                                             "(пропускає хід)");
        yield return new WaitForSeconds(1.5f);

        player.IsPlayable = false;
    }

    private IEnumerator HoldTheMatchCouroutine(Player host, Player guest, Club hostClub)
    {
        if (!hostClub.IsPlayable || hostClub.Footballer == null)
        {
            MessagePanelController.Instance.Show("Домашній клуб не може грати");
            yield return new WaitForSeconds(1.5f);
            yield break;
        }

        if (guest.Clubs.Count == 0)
        {
            MessagePanelController.Instance.Show("У гостя нема клубів");
            yield return new WaitForSeconds(1.5f);
            yield break;
        }

        var guestClub = guest.Clubs.FirstOrDefault(club => club.IsPlayable);

        // якщо вільного гостьового клубу нема, то технічна поразка
        int payment;
        if (guestClub == null)
        {
            MessagePanelController.Instance.Show("У гостя нема клубів не в запасі");
            yield return new WaitForSeconds(1.5f);
            MessagePanelController.Instance.Show($"Технічна поразка, оплата {MatchPaymentSum(hostClub)}");
            yield return new WaitForSeconds(1.5f);

            payment = MatchPaymentSum(hostClub);
            if (guest.Opponent == null) // наш гравець платить
            {
                MoneyPayer.SetPayment(payment);
                Bank.AddMoney(host, MoneyPayer.RequiredMoney);
                GameManager.StatsManager.AddToStat("expenses", (ulong)payment);
            }
            else // противник платить
            {
                bool canPay = guest.Opponent.TryResolveMoney(guest, payment);

                if (!canPay)
                {
                    Bank.AddMoney(host, (int)guest.MoneySum); // оплата, що лишилося у гравця, який платить
                    yield return DeclareBankruptcy(guest);
                    yield break;
                }

                Bank.TakeMoney(guest, payment);
                Bank.AddMoney(host, payment);
            }

            yield break;
        }

        // якщо є проводиться матч
        MessagePanelController.Instance.Show($"Матч між {hostClub.Name} ({host.ColorString})" +
                                             $" проти {guestClub.Name} ({guest.ColorString})");
        yield return new WaitForSeconds(1.5f);

        // хід господаря
        MessagePanelController.Instance.Show($"Хід {hostClub.Name} ({host.ColorString})");
        yield return new WaitForSeconds(1.5f);

        int hostPoints = GameManager.ThrowDices();
        MessagePanelController.Instance.Show($"Випало: {hostPoints}");
        yield return new WaitForSeconds(1.5f);

        hostPoints += hostClub.Footballer.Points;
        MessagePanelController.Instance.Show($"Плюс {hostClub.Footballer.Points} очок гравця: {hostPoints}");
        yield return new WaitForSeconds(1.5f);

        if (hostClub.Trainer != null)
        {
            hostPoints += hostClub.Trainer.Points;
            MessagePanelController.Instance.Show($"Плюс {hostClub.Trainer.Points} очок тренера: {hostPoints}");
            yield return new WaitForSeconds(1.5f);
        }

        // хід гостя
        MessagePanelController.Instance.Show($"Хід {guestClub.Name} ({guest.ColorString})");
        yield return new WaitForSeconds(1.5f);
        int guestPoints = GameManager.ThrowDices();
        MessagePanelController.Instance.Show($"Випало: {guestPoints}");
        yield return new WaitForSeconds(1.5f);

        guestPoints += guestClub.Footballer.Points;
        MessagePanelController.Instance.Show($"Плюс {guestClub.Footballer.Points} очок гравця: {guestPoints}");
        yield return new WaitForSeconds(1.5f);

        if (guestClub.Trainer != null)
        {
            guestPoints += guestClub.Trainer.Points;
            MessagePanelController.Instance.Show($"Плюс {guestClub.Trainer.Points} очок тренера: {guestPoints}");
            yield return new WaitForSeconds(1.5f);
        }

        // вивести переможця
        Player winner = null;
        if (hostPoints > guestPoints)
        {
            winner = host;
            MessagePanelController.Instance.Show($"Переміг {hostClub.Name} ({host.ColorString}) " +
                                                 $"із рахунком {hostPoints}:{guestPoints}");
        }
        else if (hostPoints < guestPoints)
        {
            winner = guest;
            MessagePanelController.Instance.Show($"Переміг {guestClub.Name} ({guest.ColorString}) " +
                                                 $"із рахунком {hostPoints}:{guestPoints}");
        }
        else
        {
            MessagePanelController.Instance.Show("Нічия");
        }

        yield return new WaitForSeconds(1.5f);

        if (winner == host)
        {
            payment = MatchPaymentSum(hostClub);
            if (guest.Opponent == null) // наш гравець платить
            {
                MoneyPayer.SetPayment(payment);
                Bank.AddMoney(host, MoneyPayer.RequiredMoney);
                GameManager.StatsManager.AddToStat("expenses", (ulong)payment);
            }
            else // противник платить
            {
                bool canPay = guest.Opponent.TryResolveMoney(guest, payment);

                if (!canPay)
                {
                    Bank.AddMoney(host, (int)guest.MoneySum);
                    yield return DeclareBankruptcy(guest);
                    yield break;
                }

                Bank.TakeMoney(guest, payment);
                Bank.AddMoney(host, payment);
                if (winner.Opponent == null)
                {
                    GameManager.StatsManager.AddToStat("income", (ulong)payment);
                    if (GameManager.StatsManager.GetStat("maxBudget") <= winner.MoneySum)
                        GameManager.StatsManager.AddToStat("maxBudget", winner.MoneySum);
                }
            }
        }
        else if (winner == guest)
        {
            payment = MatchPaymentSum(guestClub);
            if (host.Opponent == null) // наш гравець платить
            {
                MoneyPayer.SetPayment(payment);
                Bank.AddMoney(guest, MoneyPayer.RequiredMoney);
                GameManager.StatsManager.AddToStat("expenses", (ulong)payment);
            }
            else // противник платить
            {
                bool canPay = host.Opponent.TryResolveMoney(host, payment);

                if (!canPay)
                {
                    Bank.AddMoney(guest, (int)guest.MoneySum);
                    yield return DeclareBankruptcy(guest);
                    yield break;
                }

                Bank.TakeMoney(host, payment);
                Bank.AddMoney(guest, payment);
                if (winner.Opponent == null)
                {
                    GameManager.StatsManager.AddToStat("income", (ulong)payment);
                    if (GameManager.StatsManager.GetStat("maxBudget") <= winner.MoneySum)
                        GameManager.StatsManager.AddToStat("maxBudget", winner.MoneySum);
                }
            }
        }
        
        guestClub.IsPlayable = false;
        
        MoveClubToEnd(guest, guestClub);
        
        if (winner is { Opponent: null })
        {
            GameManager.MatchStatsData.matchWins++;
        }
    }

    private void ShowPropertyWithChoiceToBuy(Cell cell, Player player)
    {
        CellManager.ShowPropertyInfoPanel(cell.CellName);

        CellManager.BuyingChoice.SetActive(true);
        
        var text = BuyingChoice.transform.Find("Text").GetComponent<TMP_Text>();
        text.text = "Придбати?";
        text.color = Color.white;
        
        // Підписуємо кнопки Так/Ні
        var buttons = CellManager.BuyingChoice.GetComponentsInChildren<UnityEngine.UI.Button>();
        foreach (var btn in buttons)
            btn.onClick.RemoveAllListeners();

        buttons[0].onClick.AddListener(() =>
        {
            PendingPurchase = GameManager.Game.Clubs.FirstOrDefault(c => c.Name == cell.CellName) as Property
                              ?? GameManager.Game.Telecompanies.First(t => t.Name == cell.CellName);
            if (player.MoneySum < PendingPurchase.Price)
            {
                text.text = "Недостатньо коштів";
                text.color = Color.red;
            }
            BuyChoice = true;
            CellManager.BuyingChoice.SetActive(false);
            CellManager.ClosePropertyInfoPanel();
        });

        buttons[1].onClick.AddListener(() =>
        {
            BuyChoice = false;
            CellManager.BuyingChoice.SetActive(false);
            CellManager.ClosePropertyInfoPanel();
            PendingPurchase = null;
        });
    }

    private int MatchPaymentSum(Club club)
    {
        var sum = club.IncomeWithPlayer;
        if (club.Trainer != null)
        {
            sum += club.IncomeWithTrainer;
            if (club.Manager != null)
            {
                sum += club.IncomeWithManager;
            }
        }

        return sum;
    }

    private IEnumerator DeclareBankruptcy(Player player)
    {
        player.IsBankrupt = true;
        player.IsPlayable = false;

        player.Clubs.Clear();
        player.Telecompanies.Clear();

        foreach (var group in player.Money)
            group.Amount = 0;

        MessagePanelController.Instance.Show(
            $"Гравець {player.ColorString} банкрот і вибуває з гри"
        );
        yield return new WaitForSeconds(1.5f);

        //  якщо це наш гравець - гра завершена
        if (player.Opponent == null)
        {
            GameManager.EndGame(false); // програш
        }
    }

    private void SetTransferButtonActive(bool active = true)
    {
        var transferButton = GameObject.Find("Canvas/TransferFlowController/TransferButton");
        transferButton.SetActive(active);
    }
    
    private void MoveClubToEnd(Player player, Club club)
    {
        if (club == null)
            return;

        int index = player.Clubs.IndexOf(club);
        if (index == -1)
            return;

        player.Clubs.RemoveAt(index);
        player.Clubs.Add(club);
    }
}