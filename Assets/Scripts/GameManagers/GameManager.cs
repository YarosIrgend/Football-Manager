using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Managers")] 
    public BoardManager BoardManager;
    public PropertyManager PropertyManager;
    public CellActionManager CellActionManager;

    [Header("Objects")] 
    public Game Game;

    public Player CurrentPlayer; // поточний гравець, який ходить
    public int CurrentPlayerIndex; // індекс поточного гравця
    private bool AreTurnConditionsCompleted; // закінчити хід можна лише після виконання умов (оплати)
    
    [Header("Buttons")]
    public GameObject MakeTurnButton;
    public GameObject EndTurnButton;
    public GameObject CloseMessagePanelButton;
    public GameObject ClosePropertyInfoPanelButton;

    [Header("Panels")] 
    public GameObject MessagePanel;
    public GameObject PropertyInfoPanel;

    [Header("Other")] 
    public GameObject CardPrefab;
    public Bank Bank;
    public MoneyPayer MoneyPayer;
    
    private void Start()
    {
        //Game.GameSettings = MatchSettingsController.GameSettings;
        Game.GameSettings = new GameSettings { Difficulty = Difficulty.Easy, PlayerCount = 4, ChipColor = Color.blue };
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
        InitializeClubs();
        InitializeTelecompanies();
        InitializeBonusesAndFines();
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
                Money = InstantiateMoney()
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

    private static List<BanknoteGroup> InstantiateMoney()
    {
        return new List<BanknoteGroup>
        {
            new BanknoteGroup
            {
                Banknote = new Banknote { BanknoteImagePath = null, Value = 5_000_000 },
                Amount = 2
            },
            new BanknoteGroup
            {
                Banknote = new Banknote { BanknoteImagePath = null, Value = 2_000_000 },
                Amount = 1
            },
            new BanknoteGroup
            {
                Banknote = new Banknote { BanknoteImagePath = null, Value = 1_000_000 },
                Amount = 0
            },
            new BanknoteGroup
            {
                Banknote = new Banknote { BanknoteImagePath = null, Value = 500_000 },
                Amount = 0
            },
            new BanknoteGroup
            {
                Banknote = new Banknote { BanknoteImagePath = null, Value = 200_000 },
                Amount = 0
            },
            new BanknoteGroup
            {
                Banknote = new Banknote { BanknoteImagePath = null, Value = 100_000 },
                Amount = 0
            }
        };
    }

    #endregion

    #region Chips

    private void SetChips()
    {
        BoardManager.SetChips(Game);
    }

    #endregion

    # region Clubs

    private void InitializeClubs()
    {
        Game.Clubs = new()
        {
            new Club
            {
                Name = "ПСЖ",
                ImagePath = "Images/Club Cards/PSG",
                Price = 2_200_000,
                IncomeWithPlayer = 500_000,
                IncomeWithTrainer = 1_500_000,
                IncomeWithManager = 8_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Ліон",
                ImagePath = "Images/Club Cards/Lyon",
                Price = 1_400_000,
                IncomeWithPlayer = 500_000,
                IncomeWithTrainer = 1_000_000,
                IncomeWithManager = 6_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Марсель",
                ImagePath = "Images/Club Cards/Marseille",
                Price = 1_400_000,
                IncomeWithPlayer = 500_000,
                IncomeWithTrainer = 1_000_000,
                IncomeWithManager = 6_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Феєноорд",
                ImagePath = "Images/Club Cards/Feyenoord",
                Price = 1_000_000,
                IncomeWithPlayer = 300_000,
                IncomeWithTrainer = 800_000,
                IncomeWithManager = 5_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "ПСВ",
                ImagePath = "Images/Club Cards/PSV",
                Price = 2_000_000,
                IncomeWithPlayer = 500_000,
                IncomeWithTrainer = 1_300_000,
                IncomeWithManager = 7_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Аякс",
                ImagePath = "Images/Club Cards/Ajax",
                Price = 1_800_000,
                IncomeWithPlayer = 400_000,
                IncomeWithTrainer = 1_100_000,
                IncomeWithManager = 6_500_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Рома",
                ImagePath = "Images/Club Cards/Roma",
                Price = 2_600_000,
                IncomeWithPlayer = 800_000,
                IncomeWithTrainer = 1_900_000,
                IncomeWithManager = 9_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Інтер",
                ImagePath = "Images/Club Cards/Inter",
                Price = 2_400_000,
                IncomeWithPlayer = 900_000,
                IncomeWithTrainer = 1_900_000,
                IncomeWithManager = 8_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Мілан",
                ImagePath = "Images/Club Cards/Milan",
                Price = 2_400_000,
                IncomeWithPlayer = 800_000,
                IncomeWithTrainer = 1_800_000,
                IncomeWithManager = 7_500_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Реал",
                ImagePath = "Images/Club Cards/Real",
                Price = 3_000_000,
                IncomeWithPlayer = 1_400_000,
                IncomeWithTrainer = 2_800_000,
                IncomeWithManager = 11_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Барселона",
                ImagePath = "Images/Club Cards/Barcelona",
                Price = 3_000_000,
                IncomeWithPlayer = 1_400_000,
                IncomeWithTrainer = 2_800_000,
                IncomeWithManager = 12_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Валенсія",
                ImagePath = "Images/Club Cards/Valencia",
                Price = 2_400_000,
                IncomeWithPlayer = 1_000_000,
                IncomeWithTrainer = 2_000_000,
                IncomeWithManager = 9_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Баварія",
                ImagePath = "Images/Club Cards/Bayern",
                Price = 2_600_000,
                IncomeWithPlayer = 1_200_000,
                IncomeWithTrainer = 2_500_000,
                IncomeWithManager = 10_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Баєр",
                ImagePath = "Images/Club Cards/Bayer",
                Price = 2_000_000,
                IncomeWithPlayer = 900_000,
                IncomeWithTrainer = 1_800_000,
                IncomeWithManager = 8_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Вердер",
                ImagePath = "Images/Club Cards/Werder",
                Price = 1_800_000,
                IncomeWithPlayer = 700_000,
                IncomeWithTrainer = 1_400_000,
                IncomeWithManager = 7_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Челсі",
                ImagePath = "Images/Club Cards/Chelsea",
                Price = 2_800_000,
                IncomeWithPlayer = 1_300_000,
                IncomeWithTrainer = 2_800_000,
                IncomeWithManager = 10_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Манчестер",
                ImagePath = "Images/Club Cards/Manchester",
                Price = 3_000_000,
                IncomeWithPlayer = 1_400_000,
                IncomeWithTrainer = 2_900_000,
                IncomeWithManager = 11_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Ліверпуль",
                ImagePath = "Images/Club Cards/Liverpool",
                Price = 3_200_000,
                IncomeWithPlayer = 1_500_000,
                IncomeWithTrainer = 3_000_000,
                IncomeWithManager = 12_000_000,
                IsPlayable = true,
                IsMortgaged = false
            }
        };
    }

    #endregion

    # region Telecompanies

    private void InitializeTelecompanies()
    {
        Game.Telecompanies = new()
        {
            new Telecompany
            {
                Name = "ESPN",
                ImagePath = "Images/Telecompany Cards/ESPN",
                Price = 2_000_000,
                IsMortgaged = false
            },
            new Telecompany
            {
                Name = "Rai Uno",
                ImagePath = "Images/Telecompany Cards/RaiUno",
                Price = 2_000_000,
                IsMortgaged = false
            },
            new Telecompany
            {
                Name = "Eurosport",
                ImagePath = "Images/Telecompany Cards/Eurosport",
                Price = 2_000_000,
                IsMortgaged = false
            },
            new Telecompany
            {
                Name = "RTL",
                ImagePath = "Images/Telecompany Cards/RTL",
                Price = 2_000_000,
                IsMortgaged = false
            }
        };
    }

    # endregion

    # region Bonuses and Fines

    private void InitializeBonusesAndFines()
    {
        Game.Bonuses = new()
        {
            new Bonus()
            {
                ImagePath = "Images/Bonus Cards/Bonus",
                Text = "",
                Value = 700_000,
            },
            new Bonus()
            {
                ImagePath = "Images/Bonus Cards/Bonus",
                Text = "",
                Value = 500_000,
            },
            new Bonus()
            {
                ImagePath = "Images/Bonus Cards/Bonus",
                Text = "",
                Value = 1_000_000,
            },
            new Bonus()
            {
                ImagePath = "Images/Bonus Cards/Bonus",
                Text = "",
                Value = 800_000,
            }
        };

        Game.Fines = new()
        {
            new Fine()
            {
                ImagePath = "Images/FineCards/Fine",
                Text = "",
                Value = 700_000,
            },
            new Fine()
            {
                ImagePath = "Images/FineCards/Fine",
                Text = "",
                Value = 500_000,
            },
            new Fine()
            {
                ImagePath = "Images/FineCards/Fine",
                Text = "",
                Value = 1_000_000,
            },
            new Fine()
            {
                ImagePath = "Images/FineCards/Fine",
                Text = "",
                Value = 800_000,
            }
        };
    }

    # endregion

    # region TurnHandler

    public void TurnPlayer()
    {
        MakeTurnButton.SetActive(false);

        //int cells = ThrowDices();
        int cells = 8;
        ShowInfoPanel($"Випало: {cells}");

        MovePlayerChip(cells);
        SetNextPlayer();
        
        EndTurnButton.SetActive(true);
    }

    public void EndPlayerTurn()
    {
        if (!AreTurnConditionsCompleted)
        {
            MoneyPayer.ConditionsWarningPanel.SetActive(true);
            return;
        }
        EndTurnButton.SetActive(false);
        StartCoroutine(OpponentsTurnsCoroutine()); // здійснення ходів противника, поки ми чекаємо
    }

    private int ThrowDices()
    {
        return Random.Range(2, 13);
    }

    // переключення ходу на наступного гравця
    private void SetNextPlayer()
    {
        if (++CurrentPlayerIndex >= Game.Players.Count)
        {
            CurrentPlayerIndex = 0;
        }

        Debug.Log(CurrentPlayerIndex);
        CurrentPlayer = Game.Players[CurrentPlayerIndex];
    }

    // перевірка чи пройшли поле старт, щоб дати бабло
    private bool StartCellPassed(Cell currentCell, Cell newCell)
    {
        return currentCell.Index >= newCell.Index;
    }

    private void MovePlayerChip(int cells)
    {
        var currentCell = CurrentPlayer.ChipBehaviour.CurrentCell;

        ShowInfoPanel($"Випало: {cells.ToString()}");

        BoardManager.MovePlayerChip(CurrentPlayer.ChipBehaviour, cells);
        var newCell = CurrentPlayer.ChipBehaviour.CurrentCell; // нова клітинка
        CellActionManager.DoActionAccordingCell(newCell, CurrentPlayer, out AreTurnConditionsCompleted); // спільний для гравця та противників

        // якщо пройшли старт, треба виплатити
        if (StartCellPassed(currentCell, newCell))
        {
            CellActionManager.Bank.AddMoney(CurrentPlayer, 500_000);
        }
    }

    private IEnumerator OpponentsTurnsCoroutine()
    {
        while (CurrentPlayerIndex != 0)
        {
            ShowInfoPanel($"Хід наступного противника: {CurrentPlayer.ColorString}");
            yield return new WaitForSeconds(1f);
            CloseMessagePanel();

            yield return StartCoroutine(OpponentTurnCoroutine());
        }

        MakeTurnButton.SetActive(true);
    }

    private IEnumerator OpponentTurnCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        //int cells = ThrowDices();
        int cells = 8;
        ShowInfoPanel($"Випало: {cells}");
        yield return new WaitForSeconds(1f);

        MovePlayerChip(cells);
        CloseMessagePanel();

        SetNextPlayer();
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

    public void ShowInfoPanel(string message)
    {
        MessagePanel.SetActive(true);
        var text = MessagePanel.transform.Find("Message").GetComponent<TextMeshProUGUI>();
        text.text = message;
    }

    public void CloseMessagePanel()
    {
        MessagePanel.SetActive(false);
    }
}