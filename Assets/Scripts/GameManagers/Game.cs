using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [HideInInspector] public GameSettings GameSettings;

    [HideInInspector] public List<Player> Players = new();
}