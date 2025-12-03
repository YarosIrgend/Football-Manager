using UnityEngine;
using System.Collections.Generic;

public class PropertyManager : MonoBehaviour
{
    private List<Player> players;

    [Header("Prefabs")] public GameObject propertyPlanePrefab; // 3D-plane кнопка
    public GameObject propertyPanelPrefab; // Canvas панель

    [Header("Settings")] public Transform boardCenter; // Центр ігрового поля
    public float distanceFromBoard = 12f; // Радіус розташування кнопок

    private Dictionary<Player, GameObject> panels = new();
    private Dictionary<Player, GameObject> planes = new();

    public void SetPlayers(List<Player> players)
    {
        this.players = players;
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
            
            var e = plane.transform.eulerAngles;
            
            switch (i)
            {
                case 0:
                    plane.transform.localPosition = new Vector3(-3.1f, 0, -18);
                    plane.transform.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case 1:
                    plane.transform.localPosition = new Vector3(-3.1f, 0, 63);
                    plane.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case 2:
                    plane.transform.localPosition = new Vector3(-66.1f, 1, 21.6f);
                    plane.transform.rotation = Quaternion.Euler(0, 270, 0);
                    break;
                default:
                    plane.transform.localPosition = new Vector3(60f, 1, 21.6f);
                    plane.transform.rotation = Quaternion.Euler(0, 90, 0);
                    break;
            }

            planes[p] = plane;

            // Додаємо скрипт клика
            var clickable = plane.AddComponent<PropertyClickable>();
            clickable.Init(p, this);

            // 2. Створюємо UI панель
            GameObject panel = Instantiate(propertyPanelPrefab, transform);
            panel.transform.LookAt(boardCenter.position);
            panel.name = $"PropertyPanel_Player{i}";
            panel.SetActive(false);
            panels[p] = panel;
        }
    }

    public void ShowPropertyPanel(Player player)
    {
        // Закриваємо всі
        foreach (var panel in panels.Values)
            panel.SetActive(false);

        // Відкриваємо панель цього гравця
        panels[player].SetActive(true);
        Debug.Log("Showing property panel");
    }
}