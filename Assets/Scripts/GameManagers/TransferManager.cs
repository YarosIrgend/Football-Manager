using System.Collections.Generic;
using UnityEngine;

public class TransferManager : MonoBehaviour
{
    public List<Footballer> Footballers = new()
    {
        new Footballer { Points = 2, Price = 1_000_000 },
        new Footballer { Points = 4, Price = 2_000_000 },
        new Footballer { Points = 6, Price = 3_000_000 },
        new Footballer { Points = 8, Price = 4_000_000 },
        new Footballer { Points = 10, Price = 5_000_000 },
    };

    public List<Trainer> Trainers = new()
    {
        new Trainer { Points = 1, Price = 1_000_000 },
        new Trainer { Points = 2, Price = 2_000_000 },
        new Trainer { Points = 3, Price = 3_000_000 },
    };

    public Manager Manager = new() { Price = 8_000_000 };
}