using UnityEngine;
using System.Collections;

public class TurnHandler : MonoBehaviour
{
    public static GameManager gameManager;

    public static int ThrowDices()
    {
        return Random.Range(2, 13);
    }

    public void StartPlayerTurn()
    {
        gameManager.StartCoroutine(PlayerTurnCoroutine());
    }

    private IEnumerator PlayerTurnCoroutine()
    {
        gameManager.MakeTurnButton.SetActive(false);

        var currentPlayer = gameManager.CurrentPlayer;

        // Пропуск ходу
        if (!currentPlayer.IsPlayable)
        {
            MessagePanelController.Instance.Show("Ви пропускаєте хід");
            yield return new WaitForSeconds(GameManager.messageDelaySeconds);

            currentPlayer.IsPlayable = true;
            SetNextPlayer();
            gameManager.StartCoroutine(OpponentsTurnsCoroutine());
            yield break;
        }

        int cells = ThrowDices();
        MessagePanelController.Instance.Show($"Випало: {cells}");
        yield return new WaitForSeconds(GameManager.messageDelaySeconds);

        gameManager.EndTurnButton.SetActive(true);
        yield return gameManager.StartCoroutine(MovePlayerChipCoroutine(cells));
    }

    public void EndPlayerTurn()
    {
        if (!gameManager.AreTurnConditionsCompleted)
        {
            gameManager.MoneyPayer.ConditionsWarningPanel.SetActive(true);
            return;
        }

        ProcessPendingPurchase();

        gameManager.EndTurnButton.SetActive(false);
        gameManager.MoneyPayer.gameObject.SetActive(false);

        SetNextPlayer();
        gameManager.StartCoroutine(OpponentsTurnsCoroutine());
    }

    private IEnumerator OpponentsTurnsCoroutine()
    {
        while (gameManager.CurrentPlayerIndex != 0)
        {
            var player = gameManager.CurrentPlayer;

            MessagePanelController.Instance
                .Show($"Хід наступного противника: {player.ColorString}");

            yield return new WaitForSeconds(GameManager.messageDelaySeconds);

            if (!player.IsPlayable)
            {
                MessagePanelController.Instance
                    .Show($"{player.ColorString} пропускає хід");

                yield return new WaitForSeconds(GameManager.messageDelaySeconds);
                player.IsPlayable = true;
                SetNextPlayer();
                continue;
            }

            yield return OpponentTurnCoroutine();
            gameManager.RemoveBankrupts();
        }

        gameManager.MakeTurnButton.SetActive(true);
    }

    private IEnumerator OpponentTurnCoroutine()
    {
        int cells = ThrowDices();

        MessagePanelController.Instance.Show($"Випало: {cells}");
        yield return new WaitForSeconds(GameManager.messageDelaySeconds);

        yield return gameManager.StartCoroutine(MovePlayerChipCoroutine(cells));
        SetNextPlayer();
    }

    private IEnumerator MovePlayerChipCoroutine(int cells)
    {
        var player = gameManager.CurrentPlayer;
        var currentCell = player.ChipBehaviour.CurrentCell;

        gameManager.BoardManager.MovePlayerChip(player.ChipBehaviour, cells);

        var newCell = player.ChipBehaviour.CurrentCell;

        if (currentCell.Index >= newCell.Index)
        {
            gameManager.Bank.AddMoney(player, 500_000);
        }

        yield return gameManager.StartCoroutine(
            gameManager.CellActionManager.DoActionAccordingCellCoroutine(
                gameManager.Game,
                newCell,
                player,
                completed => gameManager.AreTurnConditionsCompleted = completed
            )
        );
    }

    private void SetNextPlayer()
    {
        if (++gameManager.CurrentPlayerIndex >= gameManager.Game.Players.Count)
            gameManager.CurrentPlayerIndex = 0;

        gameManager.CurrentPlayer =
            gameManager.Game.Players[gameManager.CurrentPlayerIndex];
    }

    private void ProcessPendingPurchase()
    {
        var pending = gameManager.CellActionManager.PendingPurchase;
        if (pending == null) return;

        switch (pending)
        {
            case Club club:
                gameManager.CurrentPlayer.Clubs.Add(club);
                break;
            case Telecompany tele:
                gameManager.CurrentPlayer.Telecompanies.Add(tele);
                break;
        }

        gameManager.CellActionManager.PendingPurchase = null;
    }
}
