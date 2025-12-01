using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    private BoardManager boardManager;

    [Header("Chip Prefabs")] 
    public GameObject BlueChipPrefab;
    public GameObject RedChipPrefab;
    public GameObject GreenChipPrefab;
    public GameObject YellowChipPrefab;

    [HideInInspector] public GameSettings GameSettings;

    [HideInInspector] public List<Player> Players = new();

    [Obsolete("Obsolete")]
    private void Start()
    {
        //GameSettings = MatchSettingsController.GameSettings;
        GameSettings = new() { Difficulty = Difficulty.Easy, PlayerCount = 4, ChipColor = Color.blue };
        boardManager = FindObjectOfType<BoardManager>();
        InitializePlayers();
        SetChips();
    }

    #region Players

    private void InitializePlayers()
    {
        for (int i = 0; i < GameSettings.PlayerCount; i++)
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
                player.ChipColor = GameSettings.ChipColor;
                player.opponent = null;
            }
            else
            {
                // Противник
                player.ChipColor = SetColorRandomlyExclusively();
                player.opponent = GameSettings.Difficulty == Difficulty.Easy
                    ? new EasyOpponent()
                    : new HardOpponent();
            }

            Players.Add(player);
        }
    }

    private Color SetColorRandomlyExclusively()
    {
        List<Color> takenColors = Players.Select(p => p.ChipColor).ToList();

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
            { new Banknote { BanknoteImagePath = null, Value = 100_000 }, 4 },
            { new Banknote { BanknoteImagePath = null, Value = 200_000 }, 3 },
            { new Banknote { BanknoteImagePath = null, Value = 500_000 }, 2 },
            { new Banknote { BanknoteImagePath = null, Value = 1_000_000 }, 1 },
            { new Banknote { BanknoteImagePath = null, Value = 2_000_000 }, 1 },
            { new Banknote { BanknoteImagePath = null, Value = 5_000_000 }, 2 }
        };
    }

    #endregion

    #region Chips

    private void SetChips()
    {
        foreach (var player in Players)
        {
            GameObject prefab = GetPrefabForColor(player.ChipColor);
            if (prefab == null)
            {
                Debug.LogError("Prefab for color " + player.ChipColor + " is null!");
                continue;
            }

            GameObject chipGO = Instantiate(prefab);

            // Додаємо компонент Chip, якщо його немає на префабі
            Chip chip = chipGO.GetComponent<Chip>();
            if (chip == null)
                chip = chipGO.AddComponent<Chip>();

            chip.Color = player.ChipColor;
            player.ChipBehaviour = chip;

            // Ставимо на стартову клітинку
            PlaceChipOnCell(chip, boardManager.Board.cells[0]);
        }
    }

    private GameObject GetPrefabForColor(Color color)
    {
        if (color == Color.blue) return BlueChipPrefab;
        if (color == Color.red) return RedChipPrefab;
        if (color == Color.green) return GreenChipPrefab;
        if (color == Color.yellow) return YellowChipPrefab;
        return null;
    }

    private void PlaceChipOnCell(Chip chip, Cell cell)
    {
        SnapPoint sp = cell.GetFreeSnapPoint();

        sp.IsBusy = true;
        chip.CurrentCell = cell;
        chip.CurrentSnapPoint = sp.Point;

        chip.transform.position = sp.Point.position;
        chip.transform.localScale = Vector3.one * 20;
        Debug.Log(chip.transform.position);
    }

    #endregion
}