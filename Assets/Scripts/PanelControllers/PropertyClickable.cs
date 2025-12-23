using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PropertyClickable : MonoBehaviour, IPointerClickHandler
{
    private Player player;
    private Action<Player> DoAction;
    
    public void Init(Player p, Action<Player> doAction)
    {
        player = p;
        this.DoAction = doAction;
    }

    public void OnMouseDown()
    {
        DoAction(player);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        DoAction?.Invoke(player);
    }
}