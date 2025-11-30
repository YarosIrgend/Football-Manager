using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    public List<Club> Clubs;
    public List<Telecompany> Telecompanies;
    public Chip chip;
    public Dictionary<Banknote, int> Money; 
    public Opponent opponent;

    public int MoneySum
    {
        get
        {
            return Money.Sum(banknoteGroup => banknoteGroup.Key.Value * banknoteGroup.Value);
        }
    }
}