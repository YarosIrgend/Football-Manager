using UnityEngine;
using UnityEngine.EventSystems;

public class TransferButton : MonoBehaviour, IPointerDownHandler
{
    public TransferFlowController TransferFlowController;
    public GameObject TransferChoicePanel;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        TransferFlowController.TransferPanelController.CloseAll();
        TransferChoicePanel.SetActive(true);
    }
}