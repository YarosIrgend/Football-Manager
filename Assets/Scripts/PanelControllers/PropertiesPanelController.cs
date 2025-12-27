// загально для клубів та телекомпаній

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesPanelController : MonoBehaviour
{
    public GameObject ClubsPlane;
    public GameObject TelecompaniesPlane;

    public GameObject ClubsPanel; // спільний для клубів та теле-ній 
    public Transform ClubsContent;
    public GameObject ClubCardPrefab;

    public GameObject ClubInfoPanel;

    public void CloseClubsPanel()
    {
        var scrollView = ClubsPanel.transform.Find("Scroll View").gameObject;
        scrollView.SetActive(false);
        ClubsPanel.SetActive(false);
    }

    public void CloseClubInfoPanel()
    {
        ClubInfoPanel.SetActive(false);
        ClearClubInfo();
    }

    # region Clubs

    public void ShowClubsPanel(Player player)
    {
        // Закрити панелі
        ClubInfoPanel.SetActive(false);
        ClubsPanel.SetActive(false);
        var scrollView = ClubsPanel.transform.Find("Scroll View").gameObject;
        scrollView.SetActive(false);

        ClubsPanel.SetActive(true);
        var textObj = ClubsPanel.transform.Find("NoClubsText").gameObject;
        var tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = "Нема клубів";

        if (player.Clubs.Count == 0)
        {
            textObj.SetActive(true);
            return;
        }

        textObj.SetActive(false);

        ShowClubs(player);
    }

    private void ShowClubs(Player player)
    {
        ClearClubsAndTelecompanies();
        var scrollView = ClubsPanel.transform.Find("Scroll View").gameObject;
        scrollView.SetActive(true);

        if (player.Clubs.Count >= 5)
        {
            var scrollBar = scrollView.transform.Find("Scrollbar").gameObject;
            scrollBar.SetActive(true);
        }

        foreach (var club in player.Clubs)
        {
            GameObject card = Instantiate(ClubCardPrefab, ClubsContent);
            card.name = club.Name;
            card.SetActive(true);

            var image = card.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(club.ImagePath);

            var clickable = card.GetComponent<PropertyClickable>();
            clickable.Init(player, _ => ShowClubInfo(club));
        }
    }

    private void ShowClubInfo(Club club)
    {
        Debug.Log($"Open club: {club.Name}");
        ClubInfoPanel.SetActive(true);

        GameObject card = Instantiate(ClubCardPrefab, ClubInfoPanel.transform); // карта
        card.name = "Image";
        card.SetActive(true);
        var image = card.GetComponent<Image>(); // зображення
        image.sprite = Resources.Load<Sprite>(club.ImagePath);
        var rt = card.GetComponent<RectTransform>(); // розмір
        rt.sizeDelta = new Vector2(400f, 400f);

        SetClubData(club);
    }

    private void SetClubData(Club club)
    {
        Transform clubInfo = ClubInfoPanel.transform.Find("ClubInfo");

        CreateRow("NameRow", clubInfo, $"Назва:   {club.Name}");
        CreateRow("PriceRow", clubInfo, $"Ціна:   {club.Price}");
        CreateRow("MortgagePriceRow", clubInfo, $"Ціна закладення:   {club.Price / 2}");
        CreateRow("IncomeWithPlayerRow", clubInfo, $"Дохід з гравцем:   {club.IncomeWithPlayer}");
        CreateRow("IncomeWithTrainerRow", clubInfo, $"Дохід з тренером:   {club.IncomeWithTrainer}");
        CreateRow("IncomeWithManagerRow", clubInfo, $"Дохід з менеджером:   {club.IncomeWithManager}");

        CreateRow(
            "FootballerRow",
            clubInfo,
            $"Футболіст: {(club.Footballer != null ? $"{club.Footballer.Points}-очковий" : "Немає")}"
        );

        CreateRow(
            "TrainerRow",
            clubInfo,
            $"Тренер: {(club.Trainer != null ? $"{club.Trainer.Points}-очковий" : "Немає")}"
        );

        CreateRow(
            "ManagerRow",
            clubInfo,
            $"Менеджер: {(club.Manager != null ? "Є" : "Немає")}"
        );

        CreateRow("IsPlayableRow", clubInfo, $"Готовий до гри: {(club.IsPlayable ? "ТАК" : "НІ")}");
        CreateRow("IsMortgagedRow", clubInfo, $"Закладений: {(club.IsMortgaged ? "ТАК" : "НІ")}");
    }

    # endregion
    
    # region Telecompanies

    public void ShowTelecompaniesPanel(Player player)
    {
        // Закрити панелі
        ClubInfoPanel.SetActive(false);
        ClubsPanel.SetActive(false);
        var scrollView = ClubsPanel.transform.Find("Scroll View").gameObject;
        scrollView.SetActive(false);

        ClubsPanel.SetActive(true);
        var textObj = ClubsPanel.transform.Find("NoClubsText").gameObject;
        var tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = "Нема телекомпаній";
        tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;

        if (player.Telecompanies.Count == 0)
        {
            textObj.SetActive(true);
            return;
        }
        
        textObj.SetActive(false);

        ShowTelecompanies(player);
    }

    private void ShowTelecompanies(Player player)
    {
        ClearClubsAndTelecompanies();
        var scrollView = ClubsPanel.transform.Find("Scroll View").gameObject;
        scrollView.SetActive(true);

        foreach (var telecompany in player.Telecompanies)
        {
            GameObject card = Instantiate(ClubCardPrefab, ClubsContent);
            card.name = telecompany.Name;
            card.SetActive(true);

            var image = card.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(telecompany.ImagePath);

            var clickable = card.GetComponent<PropertyClickable>();
            clickable.Init(player, _ => ShowTelecompanyInfo(telecompany));
        }
    }

    private void ShowTelecompanyInfo(Telecompany telecompany)
    {
        ClubInfoPanel.SetActive(true);

        GameObject card = Instantiate(ClubCardPrefab, ClubInfoPanel.transform); // карта
        card.name = "Image";
        card.SetActive(true);
        var image = card.GetComponent<Image>(); // зображення
        image.sprite = Resources.Load<Sprite>(telecompany.ImagePath);
        var rt = card.GetComponent<RectTransform>(); // розмір
        rt.sizeDelta = new Vector2(400f, 400f);

        SetTelecompanyData(telecompany);
    }

    private void SetTelecompanyData(Telecompany telecompany)
    {
        // CLubInfo є спільним для клубів та теле-ній
        Transform telecompanyInfo = ClubInfoPanel.transform.Find("ClubInfo");

        CreateRow("NameRow", telecompanyInfo, $"Назва:   {telecompany.Name}");
        CreateRow("PriceRow", telecompanyInfo, $"Ціна:   {telecompany.Price}");
        CreateRow("MortgagePriceRow", telecompanyInfo, $"Ціна закладення:   {telecompany.Price / 2}");
        CreateRow("IsMortgagedRow", telecompanyInfo, $"Закладений: {(telecompany.IsMortgaged ? "ТАК" : "НІ")}");
    }

    # endregion

    private void CreateRow(string rowName, Transform parent, string text)
    {
        // ===== ROW =====
        GameObject row = new GameObject(rowName);
        row.transform.SetParent(parent, false);

        var rowRT = row.AddComponent<RectTransform>();

        // Row розтягується на всю ширину ClubInfo
        rowRT.anchorMin = new Vector2(0, 1);
        rowRT.anchorMax = new Vector2(1, 1);
        rowRT.pivot = new Vector2(0, 1);
        rowRT.sizeDelta = new Vector2(0, 32);

        // LayoutElement — КЛЮЧОВЕ
        var rowLayout = row.AddComponent<LayoutElement>();
        rowLayout.minHeight = 32;
        rowLayout.preferredHeight = 32;
        rowLayout.flexibleHeight = 0;
        rowLayout.minWidth = 0;

        // ===== DATA (TEXT) =====
        GameObject dataObj = new GameObject("Data");
        dataObj.transform.SetParent(row.transform, false);

        var tmp = dataObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 40;
        tmp.color = Color.yellow;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Left;

        // Не стискати текст
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.autoSizeTextContainer = false;

        // RectTransform тексту
        var textRT = tmp.rectTransform;
        textRT.anchorMin = new Vector2(0, 0);
        textRT.anchorMax = new Vector2(1, 1);
        textRT.pivot = new Vector2(0, 0.5f);
        textRT.offsetMin = new Vector2(10, 0); // padding зліва
        textRT.offsetMax = new Vector2(-10, 0); // padding справа
    }

    private void ClearClubInfo()
    {
        // видалити зображення
        var clubImage = ClubInfoPanel.transform.Find("Image");
        Destroy(clubImage.gameObject);

        // видалити рядки
        var clubInfo = ClubInfoPanel.transform.Find("ClubInfo");
        foreach (Transform child in clubInfo)
            Destroy(child.gameObject);
    }

    private void ClearClubsAndTelecompanies()
    {
        foreach (Transform child in ClubsContent)
            Destroy(child.gameObject);
    }
}