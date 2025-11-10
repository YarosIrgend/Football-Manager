using UnityEngine;

public class MenuPanelsController : MonoBehaviour
{
    public GameObject MainMenuPanel;
    public GameObject RulesPanel;

    public void OpenRules()
    {
        MainMenuPanel.SetActive(false);
        RulesPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        RulesPanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
