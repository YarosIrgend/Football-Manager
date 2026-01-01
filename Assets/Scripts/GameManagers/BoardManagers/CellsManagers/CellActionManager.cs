using System;
using System.Collections;
using System.Linq;
using UnityEditor.VersionControl;
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
        if (owner != null && owner != player)
        {
            MessagePanelController.Instance.Show(
                $"Є гравець {owner.ColorString}, який володіє цим клубом.");
            yield return new WaitForSeconds(1.5f);

            var ownerClub = owner.Clubs.FirstOrDefault(t => t.Name == cell.CellName); 
            // якщо клуб є, треба провести матч за можливості
            yield return StartCoroutine(HoldTheMatchCouroutine(owner, player, ownerClub));

            onCompleted(false);
        }

        // якщо ніхто не володіє, то можна придбати за бажанням
        else
        {
            if (player.Opponent == null)
            {
                ShowPropertyWithChoiceToBuy(cell);
                // Чекаємо натискання кнопки
                while (CellManager.BuyingChoice.activeSelf)
                    yield return null;

                if (BuyChoice)
                {
                    MoneyPayer.RequiredMoney = club!.Price;
                    MoneyPayer.MoneyPayerObject.SetActive(true);
                    MoneyPayer.MoneyPayerPanel.SetActive(true);
                    MoneyPayer.ShowMoney();
                    onCompleted(false);
                }
                else
                {
                    onCompleted(true);
                }
            }
            else
            {
                // Противник → автоматична обробка
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
        if (owner != null && owner != player)
        {
            MessagePanelController.Instance.Show(
                $"Є гравець {owner.ColorString}, який володіє цією компанією. Усього: {owner.Telecompanies.Count}");
            yield return new WaitForSeconds(1.5f);
            MoneyPayer.RequiredMoney = owner.Telecompanies.Count switch
            {
                1 => 300_000,
                2 => 500_000,
                3 => 1_000_000,
                _ => 2_000_000
            };
            if (player.Opponent == null) // наш гравець платить
            {
                MoneyPayer.MoneyPayerObject.SetActive(true);
                MoneyPayer.MoneyPayerPanel.SetActive(true);
                MoneyPayer.ShowMoney();
                Bank.AddMoney(owner, MoneyPayer.RequiredMoney);
                onCompleted(false);
            }
            else // противник платить
            {
                var payment = MoneyPayer.RequiredMoney;
                Bank.TakeMoney(player, payment);
                Bank.AddMoney(owner, payment);
            }
        }

        // якщо ніхто не володіє, то можна придбати за бажанням
        else
        {
            if (player.Opponent == null)
            {
                ShowPropertyWithChoiceToBuy(cell);
                // Чекаємо натискання кнопки
                while (CellManager.BuyingChoice.activeSelf)
                    yield return null;

                if (BuyChoice)
                {
                    MoneyPayer.RequiredMoney = telecompany!.Price;
                    MoneyPayer.MoneyPayerObject.SetActive(true);
                    MoneyPayer.MoneyPayerPanel.SetActive(true);
                    MoneyPayer.ShowMoney();
                    onCompleted(false);
                }
                else
                {
                    onCompleted(true);
                }
            }
            else
            {
                // Противник → автоматична обробка
                onCompleted(true);
            }
        }
    }

    private IEnumerator HandleBonusCoroutine(Player player, Game game)
    {
        var bonus = game.Bonuses.GetRandomItem();

        MessagePanelController.Instance.Show($"Бонус: {bonus.Value}");
        yield return new WaitForSeconds(1.5f);

        Bank.AddMoney(player, bonus.Value);
    }

    private IEnumerator HandleFineCoroutine(Game game, Player player, Action<bool> onCompleted)
    {
        var fine = game.Fines.GetRandomItem();

        MessagePanelController.Instance.Show($"Штраф: {fine.Value}");
        yield return new WaitForSeconds(1.5f);

        if (player.Opponent == null)
        {
            MoneyPayer.RequiredMoney = fine.Value;
            MoneyPayer.MoneyPayerObject.SetActive(true);
            MoneyPayer.MoneyPayerPanel.SetActive(true);
            MoneyPayer.ShowMoney();
            onCompleted(false);
        }
        else
        {
            Bank.TakeMoney(player, fine.Value);
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
            MoneyPayer.RequiredMoney = tax;
            MoneyPayer.MoneyPayerObject.SetActive(true);
            MoneyPayer.MoneyPayerPanel.SetActive(true);
            MoneyPayer.ShowMoney();
            onCompleted(false);
        }
        else
        {
            Bank.TakeMoney(player, tax);
            onCompleted(true);
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
        if (guestClub == null)
        {
            MessagePanelController.Instance.Show("У гостя нема клубів не в запасі");
            yield return new WaitForSeconds(1.5f);
            MessagePanelController.Instance.Show($"Технічна поразка, оплата {MatchPaymentSum(hostClub)}");
            yield return new WaitForSeconds(1.5f);

            MoneyPayer.RequiredMoney = MatchPaymentSum(hostClub);
            if (guest.Opponent == null) // наш гравець платить
            {
                MoneyPayer.MoneyPayerObject.SetActive(true);
                MoneyPayer.MoneyPayerPanel.SetActive(true);
                MoneyPayer.ShowMoney();
                Bank.AddMoney(host, MoneyPayer.RequiredMoney);
            }
            else // противник платить
            {
                var payment = MoneyPayer.RequiredMoney;
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
            MoneyPayer.RequiredMoney = MatchPaymentSum(hostClub);
            if (guest.Opponent == null) // наш гравець платить
            {
                MoneyPayer.MoneyPayerObject.SetActive(true);
                MoneyPayer.MoneyPayerPanel.SetActive(true);
                MoneyPayer.ShowMoney();
                Bank.AddMoney(host, MoneyPayer.RequiredMoney);
            }
            else // противник платить
            {
                var payment = MoneyPayer.RequiredMoney;
                Bank.TakeMoney(guest, payment);
                Bank.AddMoney(host, payment);
            }
        }
        else if (winner == guest)
        {
            MoneyPayer.RequiredMoney = MatchPaymentSum(hostClub);
            if (host.Opponent == null) // наш гравець платить
            {
                MoneyPayer.MoneyPayerObject.SetActive(true);
                MoneyPayer.MoneyPayerPanel.SetActive(true);
                MoneyPayer.ShowMoney();
                Bank.AddMoney(guest, MoneyPayer.RequiredMoney);
            }
            else // противник платить
            {
                var payment = MoneyPayer.RequiredMoney;
                Bank.TakeMoney(host, payment);
                Bank.AddMoney(guest, payment);
            }
        }
        guestClub.IsPlayable = false;
    }

    private void ShowPropertyWithChoiceToBuy(Cell cell)
    {
        // Гравець → показуємо панель з інформацією
        CellManager.ShowPropertyInfoPanel(cell.CellName);

        // Активуємо BuyingChoicePanel для ручної дії
        CellManager.BuyingChoice.SetActive(true);

        // Підписуємо кнопки "Так/Ні"
        var buttons = CellManager.BuyingChoice.GetComponentsInChildren<UnityEngine.UI.Button>();
        foreach (var btn in buttons)
            btn.onClick.RemoveAllListeners();

        buttons[0].onClick.AddListener(() =>
        {
            BuyChoice = true;
            CellManager.BuyingChoice.SetActive(false);
            CellManager.ClosePropertyInfoPanel();
            PendingPurchase = GameManager.Game.Clubs.FirstOrDefault(c => c.Name == cell.CellName) as Property
                              ?? GameManager.Game.Telecompanies.First(t => t.Name == cell.CellName);
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
}