using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [HideInInspector] public GameSettings GameSettings;

    public List<Player> Players = new();

    public List<Club> Clubs = new()
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

    public List<Telecompany> Telecompanies = new()
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

    public List<Bonus> Bonuses = new()
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

    public List<Fine> Fines = new()
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