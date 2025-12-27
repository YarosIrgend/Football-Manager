using UnityEngine;

public class CellClicker : MonoBehaviour
{
    public Cell Cell;
    public GameManager GameManager;
    
    private void OnMouseDown()
    {
        GameManager.OnCellClicked(Cell);
    }
}