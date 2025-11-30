using UnityEngine;
using System.Collections.Generic;
using System.Linq.Expressions;

public class Board : MonoBehaviour
{
    public List<Cell> cells;

    private void Start()
    {
        GenerateSnapPoints();
    }

    private List<Cell> GenerateCells()
    {
        return new List<Cell>
        {
            GenerateCell(0, "Start", CellType.Start),
            GenerateCell(1, "PSG", CellType.Club),
            GenerateCell(2, "Fine1", CellType.Fine),
            GenerateCell(3, "Lyon", CellType.Club),
            GenerateCell(4, "Marceille", CellType.Club),
            GenerateCell(5, "ESPN", CellType.Telecompany),
            GenerateCell(6, "Feyenoord", CellType.Club),
            GenerateCell(7, "PSV", CellType.Club),
            GenerateCell(8, "Taxes", CellType.Tax),
            GenerateCell(9, "Ajax", CellType.Club),
            GenerateCell(10, "Transfer1", CellType.Transfer),
            GenerateCell(11, "Roma", CellType.Club),
            GenerateCell(12, "Bonus1", CellType.Bonus),
            GenerateCell(13, "Inter", CellType.Club),
            GenerateCell(14, "RaiUno", CellType.Telecompany),
            GenerateCell(15, "Milan", CellType.Club),
            GenerateCell(16, "Transfer2", CellType.Transfer),
            GenerateCell(17, "Real", CellType.Club),
            GenerateCell(18, "Fine2", CellType.Fine),
            GenerateCell(19, "Barcelona", CellType.Club),
            GenerateCell(20, "Valencia", CellType.Club),
            GenerateCell(21, "EuroSport", CellType.Telecompany),
            GenerateCell(22, "Bayern", CellType.Club),
            GenerateCell(23, "Bayer", CellType.Club),
            GenerateCell(24, "Disqualification", CellType.Disqualification),
            GenerateCell(25, "Werder", CellType.Club),
            GenerateCell(26, "Transfer3", CellType.Transfer),
            GenerateCell(27, "Chelsea", CellType.Club),
            GenerateCell(28, "RTL", CellType.Telecompany),
            GenerateCell(29, "Manchester", CellType.Club),
            GenerateCell(30, "Bonus2", CellType.Bonus),
            GenerateCell(31, "Liverpool", CellType.Club),
        };
    }

    private Cell GenerateCell(int index, string cellName, CellType type)
    {
        GameObject cellGO = new GameObject($"Cell_{index}");
        Cell cell = cellGO.AddComponent<Cell>();

        // Заповнюємо дані
        cell.index = index;
        cell.cellName = cellName;
        cell.type = type;
        cell.snapPoints = null;

        return cell;
    }

    private void GenerateSnapPoints()
    {
        float snapOffset = 0.2f; // відстань між фішками
        foreach (var cell in cells)
        {
            cell.snapPoints = new Transform[4];

            for (int i = 0; i < 4; i++)
            {
                // Створюємо порожній GameObject для snapPoint
                GameObject snap = new GameObject("SnapPoint_" + i);
                snap.transform.parent = cell.transform;

                // Розташування точок відносно центру клітинки
                float x = (i % 2 == 0) ? -snapOffset : snapOffset;
                float z = (i < 2) ? -snapOffset : snapOffset;
                snap.transform.localPosition = new Vector3(x, 0, z);
                
                // Зберігаємо Transform у масиві клітинки
                cell.snapPoints[i] = snap.transform;

                Debug.Log(
                    $"Cell {cell.cellName}, Global: {snap.transform.position}"
                );
            }
        }
    }
}