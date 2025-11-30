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

    private void GenerateSnapPoints()
    {
        float radius = 0.1f; // відстань між фішками
        int snapCount = 4;
        
        foreach (var cell in cells)
        {
            cell.snapPoints = new SnapPoint[4];

            for (int i = 0; i < snapCount; i++)
            {
                cell.snapPoints[i] = new SnapPoint();

                GameObject snap = new GameObject("SnapPoint_" + i);
                snap.transform.parent = cell.transform;

                // Визначаємо кут для snap-точки навколо центра
                float angle = i * Mathf.PI * 2 / snapCount; // рівномірно по колу
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                float y = 0.05f;
                snap.transform.localPosition = new Vector3(x, y, z);

                cell.snapPoints[i].Point = snap.transform;
                cell.snapPoints[i].IsBusy = false;
            }
        }
    }
}