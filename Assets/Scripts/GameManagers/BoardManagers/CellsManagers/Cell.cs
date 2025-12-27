using System.Linq;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int Index; // номер клітинки на дошці
    public string CellName;
    public CellType Type;
    public SnapPoint[] SnapPoints;
    
    public SnapPoint GetFreeSnapPoint()
    {
        return SnapPoints.FirstOrDefault(sp => !sp.IsBusy);
    }
}
