using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CellManager : MonoBehaviour
{
    [Header("Dependencies")]
    public BoardManager BoardManager;
    public Game Game;

    [Header("UI")]
    public GameObject PropertyInfoPanel;
    public GameObject CardPrefab;
    public GameObject BuyingChoice;

    public void InitializeCells()
    {
        AssignClickersToCells();
    }

    #region Cells

    private void AssignClickersToCells()
    {
        foreach (var cell in BoardManager.Board.cells)
        {
            if (cell.Type is CellType.Club or CellType.Telecompany)
            {
                var clicker = cell.gameObject.AddComponent<CellClicker>();
                clicker.Cell = cell;
                clicker.CellManager = this;
            }
        }
    }

    public void OnCellClicked(Cell cell)
    {
        ShowPropertyInfoPanel(cell.CellName);
    }

    public void ShowPropertyInfoPanel(string cellName)
    {
        ClearPropertyInfo();

        var property =
            Game.Clubs.FirstOrDefault(c => c.Name == cellName) as Property ??
            Game.Telecompanies.First(t => t.Name == cellName);

        ShowPropertyInfo(property);
    }

    private void ShowPropertyInfo(Property property)
    {
        PropertyInfoPanel.SetActive(true);

        GameObject card = Instantiate(CardPrefab, PropertyInfoPanel.transform);
        card.name = "Image";

        var image = card.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>(property.ImagePath);
        image.gameObject.SetActive(true);
        
        var rt = card.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400f, 400f);

        SetPropertyData(property);
    }

    private void SetPropertyData(Property property)
    {
        Transform propertyInfo = PropertyInfoPanel.transform.Find("PropertyInfo");

        CreateRow(propertyInfo, $"Назва:   {property.Name}");
        CreateRow(propertyInfo, $"Ціна:   {property.Price:N0}");
        CreateRow(propertyInfo, $"Ціна закладення:   {property.Price / 2:N0}");

        if (property is Club club)
        {
            CreateRow(propertyInfo, $"Дохід з гравцем:   {club.IncomeWithPlayer:N0}");
            CreateRow(propertyInfo, $"Дохід з тренером:   {club.IncomeWithTrainer:N0}");
            CreateRow(propertyInfo, $"Дохід з менеджером:   {club.IncomeWithManager:N0}");
        }
    }

    private static void CreateRow(Transform parent, string text)
    {
        GameObject row = new GameObject("Row");
        row.transform.SetParent(parent, false);

        var rowRT = row.AddComponent<RectTransform>();
        rowRT.sizeDelta = new Vector2(0, 32);

        var layout = row.AddComponent<LayoutElement>();
        layout.preferredHeight = 32;

        GameObject data = new GameObject("Data");
        data.transform.SetParent(row.transform, false);

        var tmp = data.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 40;
        tmp.color = Color.yellow;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        tmp.overflowMode = TextOverflowModes.Overflow;
    }

    public void ClosePropertyInfoPanel()
    {
        PropertyInfoPanel.SetActive(false);
    }

    private void ClearPropertyInfo()
    {
        var image = PropertyInfoPanel.transform.Find("Image");
        if (image != null)
            Destroy(image.gameObject);

        var info = PropertyInfoPanel.transform.Find("PropertyInfo");
        foreach (Transform child in info)
            Destroy(child.gameObject);
    }

    #endregion
}
