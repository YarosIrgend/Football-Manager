using System;
using UnityEngine;
using UnityEngine.UI;

public class TransferFlowController : MonoBehaviour
{
    public static TransferFlowController Instance;

    public MoneyPayer MoneyPayer;
    public StatsManager StatsManager;
    public PropertiesPanelController PropertiesPanelController;
    public TransferPanelController TransferPanelController;
    public TransferManager TransferManager;
    public Bank Bank;

    private Player CurrentPlayer;
    private Action onFinish;
    
    private void Awake()
    {
        Instance = this;
    }

    public void StartTransfer(Game game, Player player, Action onFinish)
    {
        this.CurrentPlayer = player;
        this.onFinish = onFinish;

        TransferPanelController.ShowTransferChoice(
            OnBuySelected,
            OnSellSelected,
            OnNothingSelected
        );
    }

    private void OnBuySelected()
    {
        TransferPanelController.TransferChoicePanel.SetActive(false);
        TransferPanelController.TransferPanel.SetActive(true);
        PropertiesPanelController.SellButton.gameObject.SetActive(false);
        PropertiesPanelController.BuyButton.gameObject.SetActive(true);
    }

    private void OnSellSelected()
    {
        TransferPanelController.TransferChoicePanel.SetActive(false);
        PropertiesPanelController.ShowClubsPanelForTransfer(CurrentPlayer, null,
            TransferPanelController.TransferChoicePanel);
        PropertiesPanelController.BuyButton.gameObject.SetActive(false);
        PropertiesPanelController.SellButton.gameObject.SetActive(true);
    }

    private void OnNothingSelected()
    {
        TransferPanelController.TransferChoicePanel.SetActive(false);
        TransferPanelController.TransferPanel.SetActive(false);
        onFinish?.Invoke();
    }

    public void OnFootballerBuy(Footballer footballer)
    {
        if (CurrentPlayer.Clubs.Count == 0)
        {
            TransferPanelController.FootballerPanelController.InfoText.text =
                "У вас немає команд для трансферу";
            return;
        }

        if (CurrentPlayer.MoneySum < footballer.Price)
        {
            TransferPanelController.FootballerPanelController.InfoText.text =
                "Недостатньо коштів для покупки цього гравця";
            return;
        }

        TransferPanelController.FootballerPanel.SetActive(false);

        PropertiesPanelController.pendingMember = footballer;
        
        PropertiesPanelController.BuyButton.gameObject.SetActive(true);
        
        PropertiesPanelController.OnBuyFootballerToClub = (f, club) =>
        {
            if (club.Footballer != null)
            {
                MessagePanelController.Instance.Show("В цьому клубі є футболіст");
                return;
            }
            MoneyPayer.SetPayment(footballer.Price);
            StatsManager.AddToStat("expenses", (ulong)footballer.Price);
            club.Footballer = f;
        };
        
        PropertiesPanelController.ShowClubsPanelForTransfer(CurrentPlayer, footballer,
            TransferPanelController.FootballerPanel);
        
        TransferPanelController.CloseAll();
    }

    public void OnTrainerBuy(Trainer trainer)
    {
        if (CurrentPlayer.Clubs.Count == 0)
        {
            TransferPanelController.TrainerPanelController.InfoText.text =
                "У вас немає команд для трансферу";
            return;
        }

        if (CurrentPlayer.MoneySum < trainer.Price)
        {
            TransferPanelController.TrainerPanelController.InfoText.text =
                "Недостатньо коштів для покупки цього тренера";
            return;
        }

        TransferPanelController.TrainerPanel.SetActive(false);

        PropertiesPanelController.pendingMember = trainer;
        PropertiesPanelController.ShowClubsPanelForTransfer(CurrentPlayer, trainer,
            TransferPanelController.TrainerPanel);

        PropertiesPanelController.OnBuyTrainerToClub = (t, club) =>
        {
            club.Trainer = null;
            if (club.Footballer == null)
            {
                MessagePanelController.Instance.Show("Спочатку треба придбати футболіста");
                return;
            }
            
            if (club.Trainer != null)
            {
                MessagePanelController.Instance.Show("В цьому клубі є тренер");
                return;
            }
            MoneyPayer.SetPayment(trainer.Price);
            StatsManager.AddToStat("expenses", (ulong)trainer.Price);
            club.Trainer = t;
        };
    }

    public void OnManagerBuy(Manager manager)
    {
        if (CurrentPlayer.Clubs.Count == 0)
        {
            TransferPanelController.ManagerPanelController.InfoText.text =
                "У вас немає команд для трансферу";
            return;
        }

        if (CurrentPlayer.MoneySum < manager.Price)
        {
            TransferPanelController.ManagerPanelController.InfoText.text =
                "Недостатньо коштів для покупки менеджера";
            return;
        }

        TransferPanelController.ManagerPanel.SetActive(false);

        PropertiesPanelController.pendingMember = manager;
        PropertiesPanelController.ShowClubsPanelForTransfer(CurrentPlayer, manager,
            TransferPanelController.ManagerPanel);
        
        PropertiesPanelController.OnBuyManagerToClub = (m, club) =>
        {
            if (club.Trainer == null)
            {
                MessagePanelController.Instance.Show("Спочатку купіть тренера");
                return;
            }
            if (club.Manager != null)
            {
                MessagePanelController.Instance.Show("В цьому клубі є менеджер");
                return;
            }
            
            MoneyPayer.SetPayment(manager.Price);
            StatsManager.AddToStat("expenses", (ulong)manager.Price);
            club.Manager = m;
        };
    }
    
    public void SellFootballer(Club club)
    {
        Debug.Log("Продано футболіста");
        if (club.Footballer == null)
        {
            MessagePanelController.Instance.Show("У цьому клубі немає футболіста");
            return;
        }

        Bank.AddMoney(CurrentPlayer, club.Footballer.Price);
        StatsManager.AddToStat("income", (ulong)club.Footballer.Price);
        club.Footballer = null;
    }

    public void SellTrainer(Club club)
    {
        if (club.Trainer == null)
        {
            MessagePanelController.Instance.Show("У цьому клубі немає тренера");
            return;
        }

        Bank.AddMoney(CurrentPlayer, club.Trainer.Price);
        StatsManager.AddToStat("income", (ulong)club.Trainer.Price);
        club.Trainer = null;
    }

    public void SellManager(Club club)
    {
        if (club.Manager == null)
        {
            MessagePanelController.Instance.Show("У цьому клубі немає менеджера");
            return;
        }

        Bank.AddMoney(CurrentPlayer, club.Manager.Price);
        StatsManager.AddToStat("income", (ulong)club.Manager.Price);
        club.Manager = null;
    }
}