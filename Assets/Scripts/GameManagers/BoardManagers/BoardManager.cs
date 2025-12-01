using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Board Board;

    private void Start()
    {
        GenerateSnapPoints();
    }

    public void GenerateSnapPoints()
    {
        float radius = 0.25f;
        int snapCount = 4;

        foreach (var cell in Board.cells)
        {
            cell.snapPoints = new SnapPoint[snapCount];

            for (int i = 0; i < snapCount; i++)
            {
                cell.snapPoints[i] = new SnapPoint();

                GameObject snap = new GameObject("SnapPoint_" + i);
                snap.transform.SetParent(cell.transform);

                float angle = i * Mathf.PI * 2 / snapCount;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                snap.transform.localPosition = new Vector3(x, -0.1f, z);

                cell.snapPoints[i].Point = snap.transform;
                cell.snapPoints[i].IsBusy = false;
            }
        }
    }
}
