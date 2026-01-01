using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Managers")] public BoardManager BoardManager;
    public PropertyManager PropertyManager;
    public CellActionManager CellActionManager;

    [Header("Objects")] public Game Game;

    public Player CurrentPlayer; // поточний гравець, який ходить
    public int CurrentPlayerIndex; // індекс поточного гравця
    private bool AreTurnConditionsCompleted; // закінчити хід можна лише після виконання умов (оплати)

    [Header("Buttons")] public GameObject MakeTurnButton;
    public GameObject EndTurnButton;
    public GameObject CloseMessagePanelButton;
    public GameObject ClosePropertyInfoPanelButton;

    [Header("Panels")] public GameObject PropertyInfoPanel;

    [Header("Other")] public GameObject CardPrefab;
    public Bank Bank;
    public MoneyPayer MoneyPayer;
    public MessagePanelController MessagePanelController;

    private void Start()
    {
        //Game.GameSettings = MatchSettingsController.GameSettings;
        Game.GameSettings = new GameSettings { Difficulty = Difficulty.Easy, PlayerCount = 3, ChipColor = Color.blue };
        BoardManager.GenerateSnapPoints();
        InitializeGame();
        PropertyManager.SetPlayers(Game.Players);
        BoardManager.CellManager.InitializeCells();
        CurrentPlayer = Game.Players[CurrentPlayerIndex]; // наш гравець перший ходить 
        SetPlayerToMoneyExchanger();
        SetBanknoteClickablesToExchanger();
        SetPlayerToMoneyPayer();
        SetBanknoteClickablesToPayer();
    }

    private void InitializeGame()
    {
        InitializePlayers();
        SetChips();
    }

    #region Players

    private void InitializePlayers()
    {
        for (int i = 0; i < Game.GameSettings.PlayerCount; i++)
        {
            Player player = new Player
            {
                Clubs = new List<Club>(),
                Telecompanies = new List<Telecompany>(),
            };

            if (i == 0)
            {
                // Гравець користувача
                player.ChipColor = Game.GameSettings.ChipColor;
                player.Opponent = null;
            }
            else
            {
                // Противник
                player.ChipColor = SetColorRandomlyExclusively();
                player.Opponent = Game.GameSettings.Difficulty == Difficulty.Easy
                    ? new EasyOpponent()
                    : new HardOpponent();
            }

            Game.Players.Add(player);
        }
    }

    private Color SetColorRandomlyExclusively()
    {
        List<Color> takenColors = Game.Players.Select(p => p.ChipColor).ToList();

        Color color;
        do
        {
            color = SetRandomColor();
        } while (takenColors.Contains(color));

        return color;
    }

    private Color SetRandomColor()
    {
        int r = Random.Range(1, 5);
        return r switch
        {
            1 => Color.red,
            2 => Color.blue,
            3 => Color.green,
            _ => Color.yellow
        };
    }

    #endregion

    #region Chips

    private void SetChips()
    {
        BoardManager.SetChips(Game);
    }

    #endregion

    # region TurnHandler

    public static int ThrowDices()
    {
        return Random.Range(2, 13);
    }

    public void TurnPlayer()
    {
        StartCoroutine(TurnPlayerCoroutine());
    }

    public IEnumerator TurnPlayerCoroutine()
    {
        MakeTurnButton.SetActive(false);

        // наш гравець пропускає хід
        if (!CurrentPlayer.IsPlayable)
        {
            MessagePanelController.Instance.Show("Ви пропускаєте хід");
            yield return new WaitForSeconds(1.5f);

            CurrentPlayer.IsPlayable = true;

            SetNextPlayer();
            StartCoroutine(OpponentsTurnsCoroutine());
            yield break;
        }

        int cells = 24;
        //int cells = ThrowDices();
        MessagePanelController.Instance.Show($"Випало: {cells}");
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(MovePlayerChipCoroutine(cells));

        EndTurnButton.SetActive(true);
    }


    public void EndPlayerTurn()
    {
        if (!AreTurnConditionsCompleted)
        {
            MoneyPayer.ConditionsWarningPanel.SetActive(true);
            return;
        }

        // якщо була купівля клубу або телекомпанії
        if (CellActionManager.PendingPurchase != null)
        {
            var property = CellActionManager.PendingPurchase;

            switch (property)
            {
                case Club club:
                    CurrentPlayer.Clubs.Add(club);
                    break;
                case Telecompany tele:
                    CurrentPlayer.Telecompanies.Add(tele);
                    break;
            }

            CellActionManager.PendingPurchase = null; // очищаємо після покупки
        }

        EndTurnButton.SetActive(false);
        SetNextPlayer();
        StartCoroutine(OpponentsTurnsCoroutine()); // здійснення ходів противника
    }

    private IEnumerator OpponentsTurnsCoroutine()
    {
        while (CurrentPlayerIndex != 0)
        {
            MessagePanelController.Instance.Show($"Хід наступного противника: {CurrentPlayer.ColorString}");
            yield return new WaitForSeconds(1.5f);

            if (!CurrentPlayer.IsPlayable)
            {
                MessagePanelController.Instance.Show($"{CurrentPlayer.ColorString} пропускає хід");
                yield return new WaitForSeconds(1.5f);
                CurrentPlayer.IsPlayable = true;
                SetNextPlayer();
                MessagePanelController.Instance.Show($"Хід наступного противника: {CurrentPlayer.ColorString}");
                yield return new WaitForSeconds(1.5f);
            }

            yield return StartCoroutine(OpponentTurnCoroutine());
        }

        MakeTurnButton.SetActive(true);
    }

    private IEnumerator OpponentTurnCoroutine()
    {
        var cells = 1;

        MessagePanelController.Instance.Show($"Випало: {cells}");
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(MovePlayerChipCoroutine(cells));

        SetNextPlayer();
    }

    // переключення ходу на наступного гравця
    private void SetNextPlayer()
    {
        if (++CurrentPlayerIndex >= Game.Players.Count)
        {
            CurrentPlayerIndex = 0;
        }

        CurrentPlayer = Game.Players[CurrentPlayerIndex];
    }

    // перевірка чи пройшли поле старт, щоб дати бабло
    private bool StartCellPassed(Cell currentCell, Cell newCell)
    {
        return currentCell.Index >= newCell.Index;
    }

    private IEnumerator MovePlayerChipCoroutine(int cells)
    {
        var currentCell = CurrentPlayer.ChipBehaviour.CurrentCell;

        BoardManager.MovePlayerChip(CurrentPlayer.ChipBehaviour, cells);
        var newCell = CurrentPlayer.ChipBehaviour.CurrentCell;

        // дія клітинки
        yield return StartCoroutine(
            CellActionManager.DoActionAccordingCellCoroutine(
                Game,
                newCell,
                CurrentPlayer,
                completed => AreTurnConditionsCompleted = completed
            )
        );

        // старт
        if (StartCellPassed(currentCell, newCell))
        {
            Bank.AddMoney(CurrentPlayer, 500_000);
        }
    }

    # endregion

    # region Bank

    // для взаємодії гравця з обміном купюр
    private void SetPlayerToMoneyExchanger()
    {
        var moneyExchanger = GameObject.Find("Canvas/MoneyExchangerObject").gameObject;
        var moneyExchangerComponent = moneyExchanger.GetComponent<MoneyExchanger>();
        moneyExchangerComponent.Player = CurrentPlayer;
    }

    private void SetPlayerToMoneyPayer()
    {
        var moneyPayer = GameObject.Find("Canvas").transform.Find("MoneyPayerObject").gameObject;

        var moneyPayerComponent = moneyPayer.GetComponent<MoneyPayer>();
        moneyPayerComponent.Player = CurrentPlayer;
    }

    private void SetBanknoteClickablesToExchanger()
    {
        var exchanger = GameObject.Find("Canvas/MoneyExchangerObject");
        var exchangerComponent = exchanger
            .GetComponent<MoneyExchanger>();

        InitPanelClickablesToExchanger(
            exchangerComponent.PlayerMoneyPanel.transform, exchangerComponent, true
        );

        InitPanelClickablesToExchanger(
            exchangerComponent.BankMoneyPanel.transform, exchangerComponent, false
        );
    }

    private void SetBanknoteClickablesToPayer()
    {
        var moneyPayer = GameObject.Find("Canvas").transform.Find("MoneyPayerObject").gameObject;

        var moneyPayerComponent = moneyPayer
            .GetComponent<MoneyPayer>();

        InitPanelClickablesToPayer(
            moneyPayerComponent.PlayerMoneyPanel.transform,
            moneyPayerComponent,
            isPlayerPanel: true
        );

        InitPanelClickablesToPayer(
            moneyPayerComponent.BankMoneyPanel.transform,
            moneyPayerComponent,
            isPlayerPanel: false
        );
    }

    private void InitPanelClickablesToPayer(Transform panel, MoneyPayer payer, bool isPlayerPanel)
    {
        string[] banknoteNames =
        {
            "Banknote5M",
            "Banknote2M",
            "Banknote1M",
            "Banknote500K",
            "Banknote200K",
            "Banknote100K"
        };

        for (int i = 0; i < banknoteNames.Length; i++)
        {
            int index = i;

            var banknoteRow = panel.Find(banknoteNames[i]);
            var image = banknoteRow.GetComponentInChildren<Image>();

            var clickable = image.gameObject.AddComponent<BanknoteClickable>();

            clickable.Action = isPlayerPanel
                ? () => payer.GiveBanknoteToBank(index, out AreTurnConditionsCompleted)
                : () => payer.TakeBanknoteFromBank(index, out AreTurnConditionsCompleted);
        }
    }

    private void InitPanelClickablesToExchanger(Transform panel, MoneyExchanger exchanger, bool isPlayerPanel)
    {
        string[] banknoteNames =
        {
            "Banknote5M",
            "Banknote2M",
            "Banknote1M",
            "Banknote500K",
            "Banknote200K",
            "Banknote100K"
        };
        for (int i = 0; i < banknoteNames.Length; i++)
        {
            int index = i;

            var banknoteRow = panel.Find(banknoteNames[i]);
            var image = banknoteRow.GetComponentInChildren<Image>();

            var clickable = image.gameObject.AddComponent<BanknoteClickable>();

            clickable.Action = isPlayerPanel
                ? () => exchanger.GiveBanknoteToBank(index)
                : () => exchanger.TakeBanknoteFromBank(index);
        }
    }

    # endregion
}