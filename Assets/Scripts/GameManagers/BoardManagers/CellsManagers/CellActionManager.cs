using UnityEngine;

public class CellActionManager : MonoBehaviour
{
    public MoneyPayer MoneyPayer;
    public GameManager GameManager;
    public Bank Bank;
    
    // спільний для гравця та противників
    // у Handles окремі обробки для гравця та противників
    public void DoActionAccordingCell(Cell cell, Player player, out bool areTurnConditionsCompleted)
    {
        areTurnConditionsCompleted = false;
        switch (cell.Type)
        {
            case CellType.Start:
                HandleStart(player);
                areTurnConditionsCompleted = true;
                break;

            case CellType.Club:
                HandleClub(cell, player, out areTurnConditionsCompleted);
                break;

            case CellType.Telecompany:
                HandleTelecompany(cell, player, out areTurnConditionsCompleted);
                break;

            case CellType.Bonus:
                HandleBonus(player);
                areTurnConditionsCompleted = true;
                break;

            case CellType.Fine:
                HandleFine(player, out areTurnConditionsCompleted);
                break;

            case CellType.Tax:
                HandleTax(player, out areTurnConditionsCompleted);
                break;
            
            case CellType.Transfer:
                break;
            
            case CellType.Disqualification:
                break;
            
        }        
    }
    
    private void HandleStart(Player player)
    {
        Bank.AddMoney(player, 500_000);
    }

    private void HandleClub(Cell cell, Player player, out bool areTurnConditionsCompleted)
    {
        // логіка покупки / матчу
        areTurnConditionsCompleted = false;
    }

    private void HandleTelecompany(Cell cell, Player player, out bool areTurnConditionsCompleted)
    {
        areTurnConditionsCompleted = false;
        
        //логіка для купівлі та давання грошей противнику
    }

    private void HandleBonus(Player player) { }

    private void HandleFine(Player player, out bool areTurnConditionsCompleted)
    {
        areTurnConditionsCompleted = false;
    }

    private void HandleTax(Player player, out bool areTurnConditionsCompleted)
    {
        MoneyPayer.RequiredMoney = 1_000_000;
        var tax = 1_000_000;
        
        if (player.Opponent == null)
        {
            MoneyPayer.MoneyPayerObject.SetActive(true);
            MoneyPayer.MoneyPayerPanel.SetActive(true);
            MoneyPayer.ShowMoney();
        }
        else
        {
            
            Bank.TakeMoney(player, tax);
            areTurnConditionsCompleted = true;
            return;
        }
        areTurnConditionsCompleted = false;
    }
}