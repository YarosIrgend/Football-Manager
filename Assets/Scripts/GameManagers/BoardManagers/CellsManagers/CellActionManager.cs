using UnityEngine;

public class CellActionManager : MonoBehaviour
{
    public Bank Bank;
    
    // спільний для гравця та противників
    // у Handles окремі обробки для гравця та противників
    public void DoActionAccordingCell(Cell cell, Player player)
    {
        switch (cell.Type)
        {
            case CellType.Start:
                HandleStart(player);
                break;

            case CellType.Club:
                HandleClub(cell, player);
                break;

            case CellType.Telecompany:
                HandleTelecompany(cell, player);
                break;

            case CellType.Bonus:
                HandleBonus(player);
                break;

            case CellType.Fine:
                HandleFine(player);
                break;

            case CellType.Tax:
                HandleTax(player);
                break;
        }        
    }
    private void HandleStart(Player player)
    {
        Bank.AddMoney(player, 500_000);
    }

    private void HandleClub(Cell cell, Player player)
    {
        // логіка покупки / оренди
    }

    private void HandleTelecompany(Cell cell, Player player)
    {
        
    }

    private void HandleBonus(Player player) { }
    private void HandleFine(Player player) { }
    private void HandleTax(Player player) { }
}