using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PropertyManager : MonoBehaviour
{
    private List<Player> players;

    [Header("Prefabs")] public GameObject propertyPlanePrefab; // 3D-plane кнопка
    public GameObject propertyPanelPrefab; // Canvas панель
    public PropertyPanelController PropertyPanelController;

    [Header("Settings")] public Transform boardCenter; // Центр ігрового поля

    private List<GameObject> openedPanels = new();
    private Dictionary<Player, GameObject> planes = new();

    public void SetPlayers(List<Player> players)
    {
        this.players = players;
        InitializePropertyPanels();
    }

    // Плейн-кнопка для відкриття майна
    public void InitializePropertyPanels()
    {
        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];

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

            planes[player] = plane;

            // Додаємо скрипт клика для кнопки включення властивостей
            SetClickableForPlayer(player, plane, ShowPropertyPanel);
        }
    }

    public void ShowPropertyPanel(Player player)
    {
        var propertiesPanel = GetObject("PropertiesPanelCanvas");
        if (propertiesPanel == null)
        {
            Debug.Log("No properties panel found");
        }
        var clubsPlane = propertiesPanel.transform.Find("PropertiesPanel/ClubsPlane").gameObject;
        var telecompaniesPlane = propertiesPanel.transform.Find("PropertiesPanel/TelecompaniesPlane").gameObject;

        // видаляємо clickables з кнопок, щоб переписати їх на іншого гравця
        RemoveClickable(clubsPlane);
        RemoveClickable(telecompaniesPlane);

        // Закриваємо всі
        HideAllPanels();

        // відкриваємо
        var propertyPanel = GetObject("PropertyPanel");
        propertyPanel.SetActive(true);
        openedPanels.Add(propertyPanel);
        PropertyPanelController.MoneyPanelController.ShowMoney(player);

        // присвоїти кнопкам Клуби та Телек-ії відповідні кліки з гравцями
        SetClickableForPlayer(player, clubsPlane, PropertyPanelController.PropertiesPanelController.ShowClubsPanel);
        SetClickableForPlayer(player, telecompaniesPlane,
            PropertyPanelController.PropertiesPanelController.ShowTelecompaniesPanel);
    }

    public void HideAllPanels()
    {
        var propertyPanel = GetObject("PropertyPanel");
        var clubsPanelCanvas = GetObject("ClubsPanelCanvas"); 
        var clubsPanel = clubsPanelCanvas.transform.Find("ClubsPanel").gameObject;
        
        propertyPanel.SetActive(false);
        clubsPanel.SetActive(false);
    }

    private void SetClickableForPlayer(Player player, GameObject plane, Action<Player> action)
    {
        var clickable = plane.AddComponent<PropertyClickable>();
        clickable.Init(player, action);
    }

    private void RemoveClickable(GameObject plane)
    {
        var clickable = plane.GetComponent<PropertyClickable>();
        if (clickable != null)
        {
            Destroy(clickable);
        }
    }
    
    private GameObject GetObject(string name)
    {
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            var result = FindInChildrenRecursive(root.transform, name);
            if (result != null)
                return result;
        }
        return null;
    }
    
    private GameObject FindInChildrenRecursive(Transform parent, string name)
    {
        if (parent.name == name)
            return parent.gameObject;

        foreach (Transform child in parent)
        {
            var found = FindInChildrenRecursive(child, name);
            if (found != null)
                return found;
        }

        return null;
    }
}