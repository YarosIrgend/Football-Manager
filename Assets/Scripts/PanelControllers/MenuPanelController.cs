using UnityEngine;

public class MenuPanelsController : PanelController
{
    public GameObject MainMenuPanel;
    public GameObject RulesPanel;
    public GameObject StatsPanel;
    public GameObject SettingsPanel;
    public GameObject MatchSettingsPanel;
    
    public void Start()
    {
        CurrentPanel = MainMenuPanel;
        OpenedPanels.Push(MainMenuPanel);
    }
    
    public void OpenStatsPanel()
    {
        GoInPanel(StatsPanel);
    }
    
    public void OpenRulesPanel()
    {
        GoInPanel(RulesPanel);
    }

    public void OpenSettingsPanel()
    {
        GoInPanel(SettingsPanel);
    }
    
    public void OpenMatchSettingsPanel()
    {
        GoInPanel(MatchSettingsPanel);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}