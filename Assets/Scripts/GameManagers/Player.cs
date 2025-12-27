using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Player
{
    public List<Club> Clubs;
    public List<Telecompany> Telecompanies;
    public Color ChipColor;   
    
    public Chip ChipBehaviour; // Ссилка на реальну фішку на сцені
    public List<BanknoteGroup> Money; 
    public Opponent Opponent; // якщо Opponent == null - це наш гравець

    public int MoneySum 
    {
        get
        {
            return Money.Sum(banknoteGroup => banknoteGroup.Banknote.Value * banknoteGroup.Amount);
        }
        set
        {
            
        }
    }
}