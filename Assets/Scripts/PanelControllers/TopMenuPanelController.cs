using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TopMenuPanelController : PanelController
{
    public bool isMoved;
    public GameObject TopMenuPanel;
    public GameManager GameManager;
    public Button MoveButton;
    private TMP_Text moveButtonText;
    
    private void Start()
    {
        moveButtonText = MoveButton.GetComponentInChildren<TMP_Text>();
    }
    
    public void ExitGame()
    {
        GameManager.EndGame(false);
        //SceneManager.LoadScene("MainMenu");
    }

    public void MovePanel()
    {
        if (!isMoved)
        {
            TopMenuPanel.transform.localPosition += new Vector3(0, -40, 0);
            isMoved = true;
            moveButtonText.text = "Згорнути";
        }

        else
        {
            TopMenuPanel.transform.localPosition += new Vector3(0, 40, 0);
            isMoved = false;
            moveButtonText.text = "Розгорнути";
        }
    }
}
