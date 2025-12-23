using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int index;
    public string cellName;
    public CellType type;
    public SnapPoint[] snapPoints;
    
    public SnapPoint GetFreeSnapPoint()
    {
        return snapPoints.FirstOrDefault(sp => !sp.IsBusy);
    }

    public string GetCellName()
    {
        return cellName;
    }
}
