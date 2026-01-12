using UnityEngine;
using UnityEngine.EventSystems;

public class CellClicker : MonoBehaviour
{
    public Cell Cell;
    public CellManager CellManager;
    
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        CellManager.OnCellClicked(Cell);
    }
}