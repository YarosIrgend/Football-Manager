using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FootballerPanelController : MonoBehaviour
{
    public static FootballerPanelController Instance;
    public GameObject FootballerPanel;
    
    [Header("Planes")]
    public PlaneButton TwoPointsPlane;
    public PlaneButton FourPointsPlane;
    public PlaneButton SixPointsPlane;
    public PlaneButton EightPointsPlane;
    public PlaneButton TenPointsPlane;

    [Header("UI")]
    public TMP_Text InfoText;
    public Button BuyButton;
    public Button CloseButton;
    
    private Footballer selectedFootballer;
    private Action<Footballer> onBuy;

    private Dictionary<PlaneButton, Footballer> mapping;

    private void Awake()
    {
        Instance = this;
        BuyButton.onClick.AddListener(OnBuyClicked);
        CloseButton.onClick.AddListener(OnCloseClicked);
    }

    public void Show(List<Footballer> footballers, Action<Footballer> onBuy)
    {
        FootballerPanel.SetActive(true);
        this.onBuy = onBuy;
        selectedFootballer = null;
        InfoText.text = "Оберіть футболіста";

        mapping = new Dictionary<PlaneButton, Footballer>
        {
            { TwoPointsPlane, footballers.Find(f => f.Points == 2) },
            { FourPointsPlane, footballers.Find(f => f.Points == 4) },
            { SixPointsPlane, footballers.Find(f => f.Points == 6) },
            { EightPointsPlane, footballers.Find(f => f.Points == 8) },
            { TenPointsPlane, footballers.Find(f => f.Points == 10) },
        };

        foreach (var pair in mapping)
        {
            var plane = pair.Key;
            var footballer = pair.Value;

            plane.OnClick = null;              // <-- КЛЮЧ
            plane.OnClick += () =>
            {
                Debug.Log($"Selected {footballer.Points}");
                OnFootballerSelected(footballer);
            };
        }
        
        gameObject.SetActive(true);
    }
    
    private void OnFootballerSelected(Footballer footballer)
    {
        selectedFootballer = footballer;
        Debug.Log($"Points: {selectedFootballer.Points}");
        InfoText.text =
            $"Очки: {footballer.Points}\n" +
            $"Ціна: {footballer.Price}";
    }

    private void OnBuyClicked()
    {
        if (selectedFootballer == null)
        {
            InfoText.text = "Спочатку оберіть футболіста";
            return;
        }

        onBuy?.Invoke(selectedFootballer);
    }

    public void OnCloseClicked()
    {
        TransferPanelController.Instance.ShowTransferPanel();
    }
}
