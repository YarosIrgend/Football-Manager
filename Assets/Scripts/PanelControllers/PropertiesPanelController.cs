// загально для клубів та телекомпаній

using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PropertiesPanelController : MonoBehaviour
{
    [Header("Dependencies")] public Bank Bank;
    public MoneyPayer MoneyPayer;
    public StatsManager StatsManager;

    public GameObject ClubsButton;
    public GameObject TelecompaniesButton;

    [Header("General panels")] public GameObject ClubsPanel; // спільний для клубів та теле-ній 
    public Transform ClubsContent;
    public GameObject ClubCardPrefab;
    public GameObject ClubInfoPanel;

    [Header("Transfer panels")] public GameObject ClubsPanelForTransfer;
    public Transform ClubsContentForTransfer;
    public GameObject ClubInfoPanelForTransfer;
    public GameObject LastOpenedPanel;

    public Action<Footballer, Club> OnBuyFootballerToClub;
    public Action<Trainer, Club> OnBuyTrainerToClub;
    public Action<Manager, Club> OnBuyManagerToClub;
    private Club selectedClub;
    private Property selectedProperty;
    private Player selectedPlayer;

    public ClubMember pendingMember;

    [Header("Buttons")] public Button BuyButton;
    public Button SellButton;
    public Button SellPropertyButton;
    public Button MortgagePropertyButton;
    public Button RedeemPropertyButton;

    # region Buttons

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
        SellPropertyButton.onClick.RemoveAllListeners();
        MortgagePropertyButton.onClick.RemoveAllListeners();
    }

    public void CloseClubsForTransferPanel()
    {
        var scrollView = ClubsPanelForTransfer.transform.Find("Scroll View").gameObject;
        scrollView.SetActive(false);
        ClubsPanelForTransfer.SetActive(false);
        LastOpenedPanel.SetActive(true);
    }

    private IEnumerator SellPropertyCoroutine()
    {
        switch (selectedProperty)
        {
            case Club club:
                selectedPlayer.Clubs.Remove(club);
                if (!club.IsMortgaged)
                {
                    Bank.AddMoney(selectedPlayer, club.Price);
                    StatsManager.AddToStat("income", club.Price);
                }
                else
                {
                    Bank.AddMoney(selectedPlayer, club.Price / 2);
                    StatsManager.AddToStat("income", club.Price / 2);
                }

                break;

            case Telecompany telecompany:
                selectedPlayer.Telecompanies.Remove(telecompany);
                if (!telecompany.IsMortgaged)
                {
                    Bank.AddMoney(selectedPlayer, telecompany.Price);
                    StatsManager.AddToStat("income", telecompany.Price);
                }
                else
                {
                    Bank.AddMoney(selectedPlayer, telecompany.Price / 2);
                    StatsManager.AddToStat("income", telecompany.Price / 2);
                }

                break;
        }

        if (StatsManager.GetStat("maxBudget") <= selectedPlayer.MoneySum)
            StatsManager.AddToStat("maxBudget", selectedPlayer.MoneySum);

        MessagePanelController.Instance.Show("Продано");
        yield return new WaitForSeconds(1.5f);
        CloseClubInfoPanel();
        CloseClubsPanel();
    }

    private IEnumerator MortgagePropertyCoroutine()
    {
        switch (selectedProperty)
        {
            case Club club:
                club.IsMortgaged = true;
                Bank.AddMoney(selectedPlayer, club.Price / 2);
                StatsManager.AddToStat("income", club.Price);
                break;
            case Telecompany telecompany:
                telecompany.IsMortgaged = true;
                Bank.AddMoney(selectedPlayer, telecompany.Price / 2);
                StatsManager.AddToStat("income", telecompany.Price / 2);
                break;
        }
    
        if (StatsManager.GetStat("maxBudget") <= selectedPlayer.MoneySum)
            StatsManager.AddToStat("maxBudget", selectedPlayer.MoneySum);
        
        MessagePanelController.Instance.Show("Закладено");
        yield return new WaitForSeconds(1.5f);
        CloseClubInfoPanel();
    }

    private void SellProperty()
    {
        StartCoroutine(SellPropertyCoroutine());
    }

    private void MortgageProperty()
    {
        StartCoroutine(MortgagePropertyCoroutine());
    }

    private void RedeemProperty()
    {
        CloseClubInfoPanel();
        CloseClubsPanel();
        var propertyPanel = GetObject("PropertyPanel");
        propertyPanel.SetActive(false);
        MoneyPayer.SetPayment(selectedProperty.Price / 2);
        selectedProperty.IsMortgaged = false;
        StatsManager.AddToStat("expenses", selectedProperty.Price / 2);
    }

    # endregion

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
            clickable.Init(player, _ => ShowClubInfo(club, player));
        }
    }

    private void ShowClubInfo(Club club, Player player)
    {
        ClubInfoPanel.SetActive(true);

        GameObject card = Instantiate(ClubCardPrefab, ClubInfoPanel.transform); // карта
        card.name = "Image";
        card.SetActive(true);
        var image = card.GetComponent<Image>(); // зображення
        image.sprite = Resources.Load<Sprite>(club.ImagePath);
        var rt = card.GetComponent<RectTransform>(); // розмір
        rt.sizeDelta = new Vector2(400f, 400f);

        SetClubData(club, player);
    }

    private void SetClubData(Club club, Player player)
    {
        if (player.Opponent == null)
        {
            selectedProperty = club;
            selectedPlayer = player;
            SellPropertyButton.gameObject.SetActive(true);
            SellPropertyButton.onClick.AddListener(SellProperty);
            if (!club.IsMortgaged)
            {
                RedeemPropertyButton.gameObject.SetActive(false);
                MortgagePropertyButton.gameObject.SetActive(true);
                MortgagePropertyButton.onClick.AddListener(MortgageProperty);
            }
            else
            {
                MortgagePropertyButton.gameObject.SetActive(false);
                RedeemPropertyButton.gameObject.SetActive(true);
                RedeemPropertyButton.onClick.AddListener(RedeemProperty);
            }
        }
        else
        {
            SellPropertyButton.gameObject.SetActive(false);
            MortgagePropertyButton.gameObject.SetActive(false);
            RedeemPropertyButton.gameObject.SetActive(false);
        }

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

    # region Clubs for Transfers

    public void ShowClubsPanelForTransfer(Player player, ClubMember member, GameObject lastPanel)
    {
        pendingMember = member; // запам'ятати футболіста/тренера/менеджера для покупки
        LastOpenedPanel = lastPanel; // фіксуємо, яку панель відкрити після закриття тої

        // Закрити інші панелі
        ClubInfoPanelForTransfer.SetActive(false);
        ClubsPanelForTransfer.SetActive(false);

        var scrollView = ClubsPanelForTransfer.transform.Find("Scroll View").gameObject;
        scrollView.SetActive(false);

        ClubsPanelForTransfer.SetActive(true);

        ShowClubsForTransfer(player);
    }

    private void ShowClubsForTransfer(Player player)
    {
        ClearClubsForTransfer();

        var scrollView = ClubsPanelForTransfer.transform.Find("Scroll View").gameObject;
        scrollView.SetActive(true);

        if (player.Clubs.Count >= 5)
        {
            var scrollBar = scrollView.transform.Find("Scrollbar").gameObject;
            scrollBar.SetActive(true);
        }

        foreach (var club in player.Clubs)
        {
            GameObject card = Instantiate(ClubCardPrefab, ClubsContentForTransfer);
            card.name = club.Name;
            card.SetActive(true);

            var image = card.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(club.ImagePath);

            var clickable = card.GetComponent<PropertyClickable>();
            clickable.Init(player, _ => ShowClubInfoForTransfer(club));
        }
    }

    private void ShowClubInfoForTransfer(Club club)
    {
        selectedClub = club;
        ClubInfoPanelForTransfer.SetActive(true);

        GameObject card = Instantiate(ClubCardPrefab, ClubInfoPanelForTransfer.transform);
        card.name = "Image";
        card.SetActive(true);

        var image = card.GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>(club.ImagePath);

        var rt = card.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400f, 400f);

        SetClubDataForTransfer(club);
    }

    private void SetClubDataForTransfer(Club club)
    {
        Transform clubInfo = ClubInfoPanelForTransfer.transform.Find("ClubInfo");

        CreateRow("NameRow", clubInfo, $"Назва:   {club.Name}");
        CreateRow("PriceRow", clubInfo, $"Ціна:   {club.Price}");
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
    }

    private void ClearClubsForTransfer()
    {
        foreach (Transform child in ClubsContentForTransfer)
            Destroy(child.gameObject);
    }

    public void CloseClubInfoPanelForTransfer()
    {
        TransferPanelController.Instance.TrainerPanel.SetActive(false);
        var image = ClubInfoPanelForTransfer.transform.Find("Image");
        if (image != null)
            Destroy(image.gameObject);

        var clubInfo = ClubInfoPanelForTransfer.transform.Find("ClubInfo");
        foreach (Transform child in clubInfo)
            Destroy(child.gameObject);

        ClubInfoPanelForTransfer.SetActive(false);
    }

    public void BuySelectedMember()
    {
        if (pendingMember == null || selectedClub == null)
            return;

        switch (pendingMember)
        {
            case Footballer footballer:
                OnBuyFootballerToClub?.Invoke(footballer, selectedClub);
                break;
            case Trainer trainer:
                OnBuyTrainerToClub?.Invoke(trainer, selectedClub);
                break;
            case Manager manager:
                OnBuyManagerToClub?.Invoke(manager, selectedClub);
                break;
        }

        CloseClubsPanelForTransfer();
    }

    public void CloseClubsPanelForTransfer()
    {
        CloseClubInfoPanelForTransfer();
        var scrollView = ClubsPanelForTransfer.transform.Find("Scroll View").gameObject;
        if (scrollView != null)
            scrollView.SetActive(false);
        ClubsPanelForTransfer.SetActive(false);
    }
    
    public void OnSellFootballerClicked()
    {
        if (selectedClub == null)
            return;

        TransferFlowController.Instance.SellFootballer(selectedClub);
    }

    public void OnSellTrainerClicked()
    {
        if (selectedClub == null)
            return;

        TransferFlowController.Instance.SellTrainer(selectedClub);
    }

    public void OnSellManagerClicked()
    {
        if (selectedClub == null)
            return;

        TransferFlowController.Instance.SellManager(selectedClub);
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
            clickable.Init(player, _ => ShowTelecompanyInfo(telecompany, player));
        }
    }

    private void ShowTelecompanyInfo(Telecompany telecompany, Player player)
    {
        ClubInfoPanel.SetActive(true);

        GameObject card = Instantiate(ClubCardPrefab, ClubInfoPanel.transform); // карта
        card.name = "Image";
        card.SetActive(true);
        var image = card.GetComponent<Image>(); // зображення
        image.sprite = Resources.Load<Sprite>(telecompany.ImagePath);
        var rt = card.GetComponent<RectTransform>(); // розмір
        rt.sizeDelta = new Vector2(400f, 400f);

        SetTelecompanyData(telecompany, player);
    }

    private void SetTelecompanyData(Telecompany telecompany, Player player)
    {
        if (player.Opponent == null)
        {
            selectedProperty = telecompany;
            selectedPlayer = player;
            SellPropertyButton.gameObject.SetActive(true);
            SellPropertyButton.onClick.AddListener(SellProperty);
            if (!telecompany.IsMortgaged)
            {
                RedeemPropertyButton.gameObject.SetActive(false);
                MortgagePropertyButton.gameObject.SetActive(true);
                MortgagePropertyButton.onClick.AddListener(MortgageProperty);
            }
            else
            {
                MortgagePropertyButton.gameObject.SetActive(false);
                RedeemPropertyButton.gameObject.SetActive(true);
                RedeemPropertyButton.onClick.AddListener(RedeemProperty);
            }
        }
        else
        {
            SellPropertyButton.gameObject.SetActive(false);
            MortgagePropertyButton.gameObject.SetActive(false);
            RedeemPropertyButton.gameObject.SetActive(false);
        }

        // CLubInfo є спільним для клубів та теле-ній
        Transform telecompanyInfo = ClubInfoPanel.transform.Find("ClubInfo");

        CreateRow("NameRow", telecompanyInfo, $"Назва:   {telecompany.Name}");
        CreateRow("PriceRow", telecompanyInfo, $"Ціна:   {telecompany.Price}");
        CreateRow("MortgagePriceRow", telecompanyInfo, $"Ціна закладення:   {telecompany.Price / 2}");
        CreateRow("IsMortgagedRow", telecompanyInfo, $"Закладений: {(telecompany.IsMortgaged ? "ТАК" : "НІ")}");
    }

    # endregion

    # region Helpers

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

    # endregion
}