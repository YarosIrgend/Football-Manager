using System;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class BanknoteClickable : MonoBehaviour, IPointerClickHandler
{
    public Action Action;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Action?.Invoke();    
    }
}