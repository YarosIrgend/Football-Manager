using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Game Game;
    public BoardManager boardManager;
    public PropertyManager propertyManager;

    private void Start()
    {
        //Game.GameSettings = MatchSettingsController.GameSettings;
        Game.GameSettings = new GameSettings { Difficulty = Difficulty.Easy, PlayerCount = 4, ChipColor = Color.blue };
        boardManager.GenerateSnapPoints();
        InitializeGame();
        propertyManager.SetPlayers(Game.Players);
        InitializeClubs();
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
                Money = InstantiateMoney()
            };

            if (i == 0)
            {
                // Гравець користувача
                player.ChipColor = Game.GameSettings.ChipColor;
                player.opponent = null;
            }
            else
            {
                // Противник
                player.ChipColor = SetColorRandomlyExclusively();
                player.opponent = Game.GameSettings.Difficulty == Difficulty.Easy
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
                Amount = 1
            },
            new BanknoteGroup
            {
                Banknote = new Banknote { BanknoteImagePath = null, Value = 500_000 },
                Amount = 2
            },
            new BanknoteGroup
            {
                Banknote = new Banknote { BanknoteImagePath = null, Value = 200_000 },
                Amount = 3
            },
            new BanknoteGroup
            {
                Banknote = new Banknote { BanknoteImagePath = null, Value = 100_000 },
                Amount = 4
            }
        };

    }

    #endregion

    #region Chips

    void SetChips()
    {
        boardManager.SetChips(Game);
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
                CardImagePath = "Images/Club Cards/PSG",
                Price = 2_000_000,
                IncomeWithPlayer = 500_000,
                IncomeWithTrainer = 1_500_000,
                IncomeWithManager = 8_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Ліон",
                CardImagePath = "Images/Club Cards/Lyon.png",
                Price = 1_500_000,
                IncomeWithPlayer = 500_000,
                IncomeWithTrainer = 1_000_000,
                IncomeWithManager = 6_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Марсель",
                CardImagePath = "Images/Club Cards/Marseiile.png",
                Price = 1_500_000,
                IncomeWithPlayer = 500_000,
                IncomeWithTrainer = 1_000_000,
                IncomeWithManager = 6_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Феєнорд",
                CardImagePath = "Images/Club Cards/Feyenoord.png",
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
                CardImagePath = "Images/Club Cards/PSV.png",
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
                CardImagePath = "Images/Club Cards/Ajax.png",
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
                CardImagePath = "Images/Club Cards/Roma.png",
                Price = 2_500_000,
                IncomeWithPlayer = 800_000,
                IncomeWithTrainer = 1_900_000,
                IncomeWithManager = 9_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Інтер",
                CardImagePath = "Images/Club Cards/Inter.png",
                Price = 2_500_000,
                IncomeWithPlayer = 900_000,
                IncomeWithTrainer = 1_900_000,
                IncomeWithManager = 8_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Мілан",
                CardImagePath = "Images/Club Cards/Milan.png",
                Price = 2_500_000,
                IncomeWithPlayer = 800_000,
                IncomeWithTrainer = 1_800_000,
                IncomeWithManager = 7_500_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Реал",
                CardImagePath = "Images/Club Cards/Real.png",
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
                CardImagePath = "Images/Club Cards/Barcelona.png",
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
                CardImagePath = "Images/Club Cards/Valencia.png",
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
                CardImagePath = "Images/Club Cards/Bayern.png",
                Price = 2_500_000,
                IncomeWithPlayer = 1_200_000,
                IncomeWithTrainer = 2_500_000,
                IncomeWithManager = 10_000_000,
                IsPlayable = true,
                IsMortgaged = false
            },
            new Club
            {
                Name = "Баєр",
                CardImagePath = "Images/Club Cards/Bayer.png",
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
                CardImagePath = "Images/Club Cards/Werder.png",
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
                CardImagePath = "Images/Club Cards/Chelsea.png",
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
                CardImagePath = "Images/Club Cards/Manchester.png",
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
                CardImagePath = "Images/Club Cards/Liverpool.png",
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
}