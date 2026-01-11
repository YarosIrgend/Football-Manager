using UnityEngine;
using System.Collections.Generic;

public class PanelController : MonoBehaviour
{
    protected Stack<GameObject> OpenedPanels = new ();
    protected GameObject CurrentPanel;
    
    protected void GoInPanel(GameObject panel)
    {
        CurrentPanel.SetActive(false);
        panel.SetActive(true);
        OpenedPanels.Push(panel);
    }
    
    public void BackInPanel()
    {
        if (OpenedPanels.Count <= 1)
            return;

        var current = OpenedPanels.Pop();
        var previous = OpenedPanels.Peek();

        current.SetActive(false);
        previous.SetActive(true);
        CurrentPanel = previous;
    }
} 

