using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static float messageDelaySeconds = 2.5f;
    
    [Header("Managers")] public BoardManager BoardManager;
    public StatsManager StatsManager;
    public MatchStatsData MatchStatsData;
    public PropertyManager PropertyManager;
    public CellActionManager CellActionManager;

    [Header("Objects")] public Game Game;

    public Player CurrentPlayer; // поточний гравець, який ходить
    public int CurrentPlayerIndex; // індекс поточного гравця
    public bool AreTurnConditionsCompleted; // закінчити хід можна лише після виконання умов (оплати)

    [Header("Buttons")] public GameObject MakeTurnButton;
    public GameObject EndTurnButton;
    public GameObject CloseMessagePanelButton;
    public GameObject ClosePropertyInfoPanelButton;

    [Header("Panels")] public GameObject PropertyInfoPanel;
    public GameObject ResultsPanel;

    [Header("Other")] public GameObject CardPrefab;
    public Bank Bank;
    public MoneyPayer MoneyPayer;
    public MessagePanelController MessagePanelController;
    private Stopwatch gameTimer = new();

    private void Start()
    {
        //Game.GameSettings = MatchSettingsController.GameSettings;
        Game.GameSettings = new GameSettings { Difficulty = Difficulty.Easy, PlayerCount = 2, ChipColor = Color.blue };
        TurnHandler.gameManager = this;
        BoardManager.GenerateSnapPoints();
        InitializeGame();
        PropertyManager.SetPlayers(Game.Players);
        BoardManager.CellManager.InitializeCells();
        CurrentPlayer = Game.Players[CurrentPlayerIndex]; // наш гравець перший ходить 
        SetPlayerToMoneyExchanger();
        SetBanknoteClickablesToExchanger();
        SetPlayerToMoneyPayer();
        SetBanknoteClickablesToPayer();

        gameTimer.Start();
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

    #region ManageMethods
    
    public void RemoveBankrupts()
    {
        var bankrupts = Game.Players
            .Where(player => player.IsBankrupt)
            .ToList();

        if (bankrupts.Count == 0)
            return;

        foreach (var player in bankrupts)
        {
            Destroy(player.ChipBehaviour.gameObject);
        }

        Game.Players.RemoveAll(player => player.IsBankrupt);

        // наш гравець перемагає
        if (Game.Players.Count == 1 && Game.Players[0].Opponent == null)
        {
            EndGame(true);
        }
    }

    public void EndGame(bool isWin)
    {
        Game.IsGameOver = true;
        if (isWin)
            StatsManager.AddToStat(Game.GameSettings.Difficulty == Difficulty.Easy ? "winsOnEasy" : "winsOnHard");
        else
            StatsManager.AddToStat("losses");

        StopAllCoroutines();

        gameTimer.Stop();
        MatchStatsData.matchTime = gameTimer.Elapsed;

        MakeTurnButton.SetActive(false);
        EndTurnButton.SetActive(false);

        ResultsPanel.SetActive(true);

        var text = ResultsPanel.GetComponentInChildren<TMP_Text>();
        text.text = isWin ? "Перемога!" : "Банкрот!";
        text.color = isWin ? Color.green : Color.red;

        SetValue("TimeRow", MatchStatsData.matchTime.ToString(@"hh\:mm\:ss"));
        SetValue("ClubsRow", MatchStatsData.clubsBought);
        SetValue("TelecompaniesRow", MatchStatsData.telecompaniesBought);
        SetValue("MatchesRow", MatchStatsData.matchWins);
        return;

        void SetValue(string rowName, object value)
        {
            var row = ResultsPanel.transform.Find(rowName);
            var rowValue = row.transform.Find("Value").GetComponent<TMP_Text>();
            rowValue.text = value.ToString();
        }
    }

    public void FinishGame()
    {
        SceneManager.LoadScene("MainMenu");
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