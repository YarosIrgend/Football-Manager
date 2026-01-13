using System;
using UnityEngine;
using UnityEngine.UI;

public class TransferPanelController : MonoBehaviour
{
    public static TransferPanelController Instance;
    
    public PropertiesPanelController PropertiesPanelController;
    public FootballerPanelController FootballerPanelController;
    public TrainerPanelController TrainerPanelController;
    public ManagerPanelController ManagerPanelController;
    
    [Header("Panels")]
    public GameObject TransferChoicePanel;
    public GameObject TransferPanel;
    public GameObject TransferSellPanel;
    
    public GameObject FootballerPanel;
    public GameObject TrainerPanel;
    public GameObject ManagerPanel;

    [Header("Planes")]
    public PlaneButton FootballerPlane;
    public PlaneButton TrainerPlane;
    public PlaneButton ManagerPlane;

    private void Awake()
    {
        Instance = this;
        FootballerPlane.OnClick = OnFootballerPlaneClicked;
        TrainerPlane.OnClick = OnTrainerPlaneClicked;
        ManagerPlane.OnClick = OnManagerPlaneClicked;
    }

    // ===== ENTRY POINTS =====

    public void ShowTransferChoice(Action onBuy, Action onSell, Action onNothing)
    {
        CloseAll();
        TransferChoicePanel.SetActive(true);

        var buttons = TransferChoicePanel.GetComponentsInChildren<Button>();
        foreach (var b in buttons)
            b.onClick.RemoveAllListeners();

        buttons[0].onClick.AddListener(() => onBuy());
        buttons[1].onClick.AddListener(() => onSell());
        buttons[2].onClick.AddListener(() => onNothing());
    }

    public void ShowTransferPanel()
    {
        CloseAll();
        TransferPanel.SetActive(true);
    }

    public void ShowTransferSellPanel()
    {
        PropertiesPanelController.ClubsPanelForTransfer.gameObject.SetActive(false);
        CloseAll();
        TransferSellPanel.SetActive(true);
        TransferSellPanel.transform.SetAsLastSibling();
    }

    public void CloseTransferSellPanel()
    { 
        TransferSellPanel.SetActive(false);
        PropertiesPanelController.ClubsPanelForTransfer.SetActive(true);
    }
    
    public void ShowFootballerPanel()
    {
        CloseAll();
        FootballerPanelController.Show(
            TransferFlowController.Instance.TransferManager.Footballers,
            TransferFlowController.Instance.OnFootballerBuy
        );
    }

    public void ShowTrainerPanel()
    {
        CloseAll();
        TrainerPanelController.Show(
            TransferFlowController.Instance.TransferManager.Trainers,
            TransferFlowController.Instance.OnTrainerBuy
        );
    }
    
    public void ShowManagerPanel()
    {
        CloseAll();
        ManagerPanelController.Show(
            TransferFlowController.Instance.TransferManager.Manager,
            TransferFlowController.Instance.OnManagerBuy
        );
    }

    public void CloseTransferPanel()
    {
        TransferPanel.SetActive(false);
        TransferChoicePanel.SetActive(true);
    }
    
    // ===== PLANE CLICKS =====

    private void OnFootballerPlaneClicked()
    {
        ShowFootballerPanel();
    }

    private void OnTrainerPlaneClicked()
    {
        ShowTrainerPanel();
    }

    private void OnManagerPlaneClicked()
    {
        ShowManagerPanel();
    }
    
    // ===== HELPERS =====

    public void CloseAll()
    {
        TransferChoicePanel.SetActive(false);
        TransferPanel.SetActive(false);
        FootballerPanel.SetActive(false);
        TrainerPanel.SetActive(false);
        ManagerPanel.SetActive(false);
    }
}
