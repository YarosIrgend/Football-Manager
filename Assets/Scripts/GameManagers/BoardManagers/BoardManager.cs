using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Board Board;
    public CellActionManager CellActionManager;
    public CellManager CellManager;
    
    [Header("Chip Prefabs")] 
    public GameObject BlueChipPrefab;
    public GameObject GreenChipPrefab;
    public GameObject RedChipPrefab;
    public GameObject YellowChipPrefab;
    
    // генерує точки для поставки фішок на клітинки
    public void GenerateSnapPoints()
    {
        float radius = 0.25f;
        int snapCount = 4;

        foreach (var cell in Board.cells)
        {
            cell.SnapPoints = new SnapPoint[snapCount];

            for (int i = 0; i < snapCount; i++)
            {
                cell.SnapPoints[i] = new SnapPoint();

                GameObject snap = new GameObject("SnapPoint_" + i);
                snap.transform.SetParent(cell.transform);

                float angle = i * Mathf.PI * 2 / snapCount;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                snap.transform.localPosition = new Vector3(x, -0.1f, z);

                cell.SnapPoints[i].Point = snap.transform;
                cell.SnapPoints[i].IsBusy = false;
            }
        }
    }
    
    #region Chips

    public void PlaceChipOnCell(Chip chip, Cell cell)
    {
        SnapPoint sp = cell.GetFreeSnapPoint();

        sp.IsBusy = true;
        chip.CurrentCell = cell;
        chip.CurrentSnapPoint = sp.Point;

        chip.transform.position = sp.Point.position;
        chip.transform.localScale = Vector3.one * 20;
    }
    
    public void SetChips(Game game)
    {
        foreach (var player in game.Players)
        {
            GameObject prefab = GetPrefabForColor(player.ChipColor);

            GameObject chipGo = Instantiate(prefab);
            
            // Додаємо компонент Chip, якщо його немає на префабі
            Chip chip = chipGo.GetComponent<Chip>();
            if (chip == null)
                chip = chipGo.AddComponent<Chip>();

            chip.Color = player.ChipColor;
            player.ChipBehaviour = chip;

            // Ставимо на стартову клітинку
            PlaceChipOnCell(chip, Board.cells[0]);
        }
    }

    private GameObject GetPrefabForColor(Color color)
    {
        if (color == Color.blue) return BlueChipPrefab;
        if (color == Color.red) return RedChipPrefab;
        if (color == Color.green) return GreenChipPrefab;
        if (color == Color.yellow) return YellowChipPrefab;
        return null;
    }
    
    public void MovePlayerChip(Chip chipBehaviour, int cellsCountToPass)
    {
        var newCellIndex = chipBehaviour.CurrentCell.Index + cellsCountToPass;
        if (newCellIndex > 31)
        {
            newCellIndex -= 32;
        }
        var newCell = Board.cells[newCellIndex];
        PlaceChipOnCell(chipBehaviour, newCell);
    }
    
    #endregion
    
    # region Cells

    public void InitializeCells()
    {
        CellManager.InitializeCells();    
    }
    
    # endregion
}
