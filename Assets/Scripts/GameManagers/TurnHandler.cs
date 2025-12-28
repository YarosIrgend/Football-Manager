// Потім використаємо якось

// using System.Collections;
// using UnityEngine;
//
// public class TurnHandler : MonoBehaviour
// {
//     [Header("Dependencies")]
//     public Game Game;
//     public BoardManager BoardManager;
//     public CellActionManager CellActionManager;
//     public GameManager GameManager;
//     
//     [Header("UI")]
//     public GameObject MakeTurnButton;
//     public GameObject EndTurnButton;
//     public GameObject MessagePanel;
//
//     public Player CurrentPlayer;
//     public int CurrentPlayerIndex;
//
//     #region Init
//
//     public void Initialize(Game game)
//     {
//         Game = game;
//         CurrentPlayerIndex = 0;
//         CurrentPlayer = Game.Players[CurrentPlayerIndex];
//         MakeTurnButton.SetActive(true);
//         EndTurnButton.SetActive(false);
//     }
//
//     #endregion
//
//     #region Turn flow
//
//     public void TurnPlayer()
//     {
//         MakeTurnButton.SetActive(false);
//         MovePlayerChip();
//         SetNextPlayer();
//         EndTurnButton.SetActive(true);
//     }
//
//     public void EndPlayerTurn()
//     {
//         EndTurnButton.SetActive(false);
//         StartCoroutine(OpponentsTurnsCoroutine());
//     }
//
//     #endregion
//
//     #region Core logic
//
//     private int ThrowDices()
//     {
//         return Random.Range(2, 13);
//     }
//
//     private void SetNextPlayer()
//     {
//         CurrentPlayerIndex++;
//
//         if (CurrentPlayerIndex >= Game.Players.Count)
//             CurrentPlayerIndex = 0;
//
//         CurrentPlayer = Game.Players[CurrentPlayerIndex];
//     }
//
//     private bool StartCellPassed(Cell currentCell, Cell newCell)
//     {
//         return currentCell.Index >= newCell.Index;
//     }
//
//     private void MovePlayerChip()
//     {
//         int cellsToPass = ThrowDices();
//         var currentCell = CurrentPlayer.ChipBehaviour.CurrentCell;
//
//         GameManager.ShowInfoPanel($"Випало: {cellsToPass}");
//
//         BoardManager.MovePlayerChip(CurrentPlayer.ChipBehaviour, cellsToPass);
//
//         var newCell = CurrentPlayer.ChipBehaviour.CurrentCell;
//         CellActionManager.DoActionAccordingCell(newCell, CurrentPlayer);
//
//         if (StartCellPassed(currentCell, newCell))
//         {
//             CellActionManager.Bank.AddMoney(CurrentPlayer, 500_000);
//         }
//     }
//
//     #endregion
//
//     #region Opponents
//
//     private IEnumerator OpponentsTurnsCoroutine()
//     {
//         while (CurrentPlayerIndex != 0)
//         {
//             GameManager.ShowInfoPanel("Хід наступного противника");
//             yield return new WaitForSeconds(1f);
//             GameManager.CloseMessagePanel();
//
//             yield return StartCoroutine(OpponentTurnCoroutine());
//         }
//
//         MakeTurnButton.SetActive(true);
//     }
//
//     private IEnumerator OpponentTurnCoroutine()
//     {
//         yield return new WaitForSeconds(0.5f);
//
//         int cells = ThrowDices();
//         GameManager.ShowInfoPanel($"Випало: {cells}");
//         yield return new WaitForSeconds(1f);
//
//         MovePlayerChip();
//         GameManager.CloseMessagePanel();
//         SetNextPlayer();
//     }
//
//     #endregion
//     
// }
