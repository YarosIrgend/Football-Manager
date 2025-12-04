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

    private static Dictionary<Banknote, int> InstantiateMoney()
    {
        return new Dictionary<Banknote, int>
        {
            { new Banknote { BanknoteImagePath = null, Value = 5_000_000 }, 2 },
            { new Banknote { BanknoteImagePath = null, Value = 2_000_000 }, 1 },
            { new Banknote { BanknoteImagePath = null, Value = 1_000_000 }, 1 },
            { new Banknote { BanknoteImagePath = null, Value = 500_000 }, 2 },
            { new Banknote { BanknoteImagePath = null, Value = 200_000 }, 3 },
            { new Banknote { BanknoteImagePath = null, Value = 100_000 }, 4 },
        };
    }

    #endregion
    
    #region Chips
    
    void SetChips()
    {
        boardManager.SetChips(Game);
    }
    
    #endregion
}