using UnityEngine;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    public List<Cell> cells;

    private void Start()
    {
        cells = GenerateCells();
    }

    private List<Cell> GenerateCells()
    {
        var startCell = GenerateCell(0, "Start", CellType.Start);
        var psgCell = GenerateCell(1, "PSG", CellType.Club);
        var fineCell1 = GenerateCell(2, "Fine1", CellType.Fine);
        var lyonCell = GenerateCell(3, "Lyon", CellType.Club);
        return new List<Cell> { startCell };
    }

    private Cell GenerateCell(int index, string cellName, CellType type)
    {
        GameObject cellGO = new GameObject($"Cell_{index}");
        Cell cell = cellGO.AddComponent<Cell>();

        // Заповнюємо дані
        cell.index = 0;
        cell.cellName = "Start";
        cell.type = CellType.Start;
        cell.snapPoints = null;
        
        return cell;
    }
    
    private void GenerateSnapPoints()
    {
        float snapOffset = 0.2f;
        foreach (var cell in cells)
        {
            cell.snapPoints = new Transform[4];

            for (int i = 0; i < 4; i++)
            {
                GameObject snap = new GameObject("SnapPoint_" + i);
                snap.transform.parent = cell.transform;

                // розташування точок відносно центру клітинки
                float x = (i % 2 == 0) ? -snapOffset : snapOffset;
                float z = (i < 2) ? -snapOffset : snapOffset;

                snap.transform.localPosition = new Vector3(x, 0, z);

                cell.snapPoints[i] = snap.transform;
            }
        }
    }
}