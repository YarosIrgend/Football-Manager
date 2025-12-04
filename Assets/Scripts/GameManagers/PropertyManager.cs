using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PropertyManager : MonoBehaviour
{
    private List<Player> players;

    [Header("Prefabs")] 
    public GameObject propertyPlanePrefab; // 3D-plane кнопка
    public GameObject propertyPanelPrefab; // Canvas панель
    public PropertyPanelController PropertyPanelController;
    
    [Header("Settings")] 
    public Transform boardCenter; // Центр ігрового поля

    private Dictionary<Player, GameObject> panels = new();
    private Dictionary<Player, GameObject> planes = new();

    public void SetPlayers(List<Player> players)
    {
        this.players = players;
        Debug.Log(players.Count);
        InitializePropertyPanels();
    }

    public void InitializePropertyPanels()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Player p = players[i];

            // 1. Створюємо plane кнопку
            var propertyManager = FindAnyObjectByType<PropertyManager>();
            GameObject plane = Instantiate(propertyPlanePrefab, propertyManager.transform, true);
            plane.name = $"PropertyPlane_Player{i}";
            plane.transform.LookAt(boardCenter.position);
            plane.SetActive(true);
            
            switch (i)
            {
                case 0:
                    plane.transform.localPosition = new Vector3(0f, 0, -40.6f);
                    plane.transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case 1:
                    plane.transform.localPosition = new Vector3(0f, 0, 40.6f);
                    plane.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 2:
                    plane.transform.localPosition = new Vector3(-63.5f, 0, 0f);
                    plane.transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;
                default:
                    plane.transform.localPosition = new Vector3(63.5f, 0, 0f);
                    plane.transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
            }

            planes[p] = plane;

            
            // Додаємо скрипт клика
            var clickable = plane.AddComponent<PropertyClickable>();
            clickable.Init(p, this);
            
            // 2. Створюємо UI панель
            var canvas = transform.parent.GetComponentInChildren<Canvas>();
            GameObject panel = Instantiate(propertyPanelPrefab, canvas.transform);
            panel.transform.LookAt(propertyPanelPrefab.transform);
            panel.name = $"PropertyPanel_Player{i}";
            panel.SetActive(false);
            panels[p] = panel;
        }
    }

    public void ShowPropertyPanel(Player player)
    {
        // Закриваємо всі
        HideAllPanels();
        Debug.Log($"{player.MoneySum}");
        panels[player].SetActive(true);
        if (PropertyPanelController == null)
        {
            Debug.LogError("PropertyPanelController is null");
        }

        if (PropertyPanelController.MoneyPanelController == null)
        {
            Debug.LogError("MoneyPanelController is null");
        }
        PropertyPanelController.MoneyPanelController.ShowMoney(player);
    }
    
    public void HideAllPanels()
    {
        foreach (var panel in panels.Values)
            panel.SetActive(false);
    }
}