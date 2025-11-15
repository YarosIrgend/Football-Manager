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
        GameObject currentPanel = OpenedPanels.Pop();
        GameObject panelToBack = OpenedPanels.Peek();
        currentPanel.SetActive(false);
        panelToBack.SetActive(true);
    }
} 

