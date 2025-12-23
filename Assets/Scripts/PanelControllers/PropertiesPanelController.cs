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

    public void ShowClubsPanel(Player player)
    {
        ClubsPanel.SetActive(false);
        
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

    public void ShowTelecompaniesPanel(Player player)
    {
        ClubsPanel.SetActive(false);
        
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
    }

    public void CloseClubsPanel()
    {
        var scrollView = ClubsPanel.transform.Find("Scroll View").gameObject;
        scrollView.SetActive(false);
        ClubsPanel.SetActive(false);
    }
    
    private void ShowClubs(Player player)
    {
        ClearClubs();
        var scrollView = ClubsPanel.transform.Find("Scroll View").gameObject;
        scrollView.SetActive(true);

        if (player.Clubs.Count >= 5)
        {
            var scrollBar = scrollView.GetComponent<Scrollbar>().gameObject;
            scrollBar.SetActive(true);
        }
        
        foreach (var club in player.Clubs)
        {
            GameObject card = Instantiate(ClubCardPrefab, ClubsContent);
            card.name = club.Name;
            card.SetActive(true);
            
            var image = card.GetComponent<Image>();
            image.sprite = Resources.Load<Sprite>(club.CardImagePath);

            var clickable = card.GetComponent<PropertyClickable>();
            clickable.Init(player, _ => ShowClubInfo(club));
        }
    }
    
    private void ClearClubs()
    {
        foreach (Transform child in ClubsContent)
            Destroy(child.gameObject);
    }
    
    private void ShowClubInfo(Club club)
    {
        Debug.Log($"Open club: {club.Name}");
        // тут відкриєш нову панель з інформацією
    }
    
}