using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchSettingsController : MonoBehaviour
{
    public static GameSettings GameSettings;

    public void Start()
    {
        GameSettings = new GameSettings
        {
            Difficulty = Difficulty.Easy,
            PlayerCount = 2,
            ChipColor = SetColorRandomly()
        };
    }
    
    public void StartMatch()
    {
        SceneManager.LoadScene("Game");
    }
    
    
    public void OnDifficultyChanged(int index)
    {
        GameSettings.Difficulty = index == 0 ?
            Difficulty.Easy :
            Difficulty.Hard;
        Debug.Log(GameSettings.Difficulty);
    }
    
    public void OnPlayerCountChanged(int index)
    {
        GameSettings.PlayerCount = (byte)(index + 2); // 0->2, 1->3, 2->4
        Debug.Log(GameSettings.PlayerCount);
    }
    
    public void OnColorSelected(int index)
    {
        GameSettings.ChipColor = index switch
        {
            1 => Color.red,
            2 => Color.blue,
            3 => Color.green,
            4 => Color.yellow,
            _ => SetColorRandomly()
        };
        Debug.Log(GameSettings.ChipColor);
    }

    private static Color SetColorRandomly()
    {
        var random = new System.Random();
        var color = random.Next(1, 4) switch
        {
            1 => Color.red,
            2 => Color.blue,
            3 => Color.green,
            _ => Color.yellow
        };
        return color;
    }
}
    // private Game InstantiateGame()
    // {
    //     var game = gameObject.AddComponent<Game>();
    //     for (int i = 0; i < GameSettings.PlayerCount; i++)
    //     {
    //         if (i == 0)
    //         {
    //             game.Players.Add(new Player
    //             {
    //                 Clubs = new List<Club>(),
    //                 Telecompanies = new List<Telecompany>(),
    //                 ChipColor = GameSettings.ChipColor,
    //                 Money = InstantiateMoney(),
    //                 opponent = null
    //             });
    //             continue;
    //         }
    //         game.Players.Add(new Player
    //         {
    //             Clubs = new List<Club>(),
    //             Telecompanies = new List<Telecompany>(),
    //             ChipColor = SetColorRandomlyExclusively(game),
    //             Money = InstantiateMoney(), 
    //             opponent = InstantiateOpponent()
    //         });
    //     }
    //     
    //     game.GameSettings = this.GameSettings;
    //     return game;
    // }
    //
    // private static Dictionary<Banknote, int> InstantiateMoney()
    // {
    //     var money = new Dictionary<Banknote, int>
    //     {
    //         { new Banknote { BanknoteImagePath = null, Value = 100_000 }, 4 },
    //         { new Banknote { BanknoteImagePath = null, Value = 200_000 }, 3 },
    //         { new Banknote { BanknoteImagePath = null, Value = 500_000 }, 2 },
    //         { new Banknote { BanknoteImagePath = null, Value = 1_000_000 }, 1 },
    //         { new Banknote { BanknoteImagePath = null, Value = 2_000_000 }, 1 },
    //         { new Banknote { BanknoteImagePath = null, Value = 5_000_000 }, 2 }
    //     };
    //     return money;
    // }
    //
    //
    // private Color SetColorRandomlyExclusively(Game game)
    // {
    //     var colorsTaken = new List<Color>{GameSettings.ChipColor};
    //     Color color;
    //     colorsTaken.AddRange(game.Players.Where(player => player.opponent is not null).Select(player => player.ChipColor));
    //
    //     do
    //     {
    //         color = SetColorRandomly();
    //     }
    //     while(colorsTaken.Contains(color));
    //     
    //     return color;
    // }
    //
    // private Opponent InstantiateOpponent()
    // {
    //     if (GameSettings.Difficulty == Difficulty.Easy)
    //     {
    //         return new EasyOpponent();
    //     }
    //
    //     return new HardOpponent();
    // }
