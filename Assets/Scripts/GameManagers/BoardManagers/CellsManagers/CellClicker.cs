using UnityEngine;

public class CellClicker : MonoBehaviour
{
    public Cell Cell;
    public CellManager CellManager;
    
    private void OnMouseDown()
    {
        CellManager.OnCellClicked(Cell);
    }
}