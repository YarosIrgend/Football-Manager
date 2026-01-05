using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerPanelController : MonoBehaviour
{
    public static ManagerPanelController Instance;
    public GameObject ManagerPanel;

    [Header("Planes")] public PlaneButton ManagerPlane;

    [Header("UI")] public TMP_Text InfoText;
    public Button BuyButton;
    public Button CloseButton;

    private Manager selectedManager;
    private Action<Manager> onBuy;

    private void Awake()
    {
        Instance = this;
        BuyButton.onClick.AddListener(OnBuyClicked);
        CloseButton.onClick.AddListener(OnCloseClicked);
    }

    public void Show(Manager manager, Action<Manager> onBuy)
    {
        ManagerPanel.SetActive(true);
        this.onBuy = onBuy;
        selectedManager = null;
        InfoText.text = "Оберіть менеджера";

        ManagerPlane.OnClick = null;
        ManagerPlane.OnClick += () => { OnManagerSelected(manager); };
        gameObject.SetActive(true);
    }

    private void OnManagerSelected(Manager manager)
    {
        selectedManager = manager;
        InfoText.text =
            $"Ціна: {manager.Price}";
    }

    private void OnBuyClicked()
    {
        if (selectedManager == null)
        {
            InfoText.text = "Спочатку оберіть менеджера";
            return;
        }

        onBuy?.Invoke(selectedManager);
    }

    public void OnCloseClicked()
    {
        TransferPanelController.Instance.ShowTransferPanel();
    }
}