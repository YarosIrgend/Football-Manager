using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainerPanelController : MonoBehaviour
{
    public static TrainerPanelController Instance;
    public GameObject TrainerPanel;

    [Header("Planes")] public PlaneButton OnePointPlane;
    public PlaneButton TwoPointsPlane;
    public PlaneButton ThreePointsPlane;

    [Header("UI")] public TMP_Text InfoText;
    public Button BuyButton;
    public Button CloseButton;

    private Trainer selectedTrainer;
    private Action<Trainer> onBuy;

    private Dictionary<PlaneButton, Trainer> mapping;

    private void Awake()
    {
        Instance = this;
        BuyButton.onClick.AddListener(OnBuyClicked);
        CloseButton.onClick.AddListener(OnCloseClicked);
    }

    public void Show(List<Trainer> trainers, Action<Trainer> onBuy)
    {
        TrainerPanel.SetActive(true);
        this.onBuy = onBuy;
        selectedTrainer = null;
        InfoText.text = "Оберіть тренера";

        mapping = new Dictionary<PlaneButton, Trainer>()
        {
            { OnePointPlane, trainers.Find(t => t.Points == 1) },
            { TwoPointsPlane, trainers.Find(t => t.Points == 2) },
            { ThreePointsPlane, trainers.Find(t => t.Points == 3) },
        };

        foreach (var pair in mapping)
        {
            var plane = pair.Key;
            var trainer = pair.Value;

            plane.OnClick = null; 
            plane.OnClick += () =>
            {
                Debug.Log($"Selected {trainer.Points}");
                OnTrainerSelected(trainer);
            };
        }

        gameObject.SetActive(true);
    }

    private void OnTrainerSelected(Trainer trainer)
    {
        selectedTrainer = trainer;
        Debug.Log($"Points: {selectedTrainer.Points}");
        InfoText.text =
            $"Очки: {trainer.Points}\n" +
            $"Ціна: {trainer.Price:N0}";
    }

    private void OnBuyClicked()
    {
        if (selectedTrainer == null)
        {
            InfoText.text = "Спочатку оберіть тренера";
            return;
        }

        onBuy?.Invoke(selectedTrainer);
    }

    public void OnCloseClicked()
    {
        TransferPanelController.Instance.ShowTransferPanel();
    }
}