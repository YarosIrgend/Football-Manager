using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [HideInInspector] public GameSettings GameSettings;

    public List<Player> Players = new();
    
    public List<Club> Clubs = new();
    
    public List<Telecompany> Telecompanies = new();
    
    public List<Bonus> Bonuses = new();
    
    public List<Fine> Fines = new();
}