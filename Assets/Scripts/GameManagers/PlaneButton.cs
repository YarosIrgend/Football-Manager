using UnityEngine;
using System;

public class PlaneButton : MonoBehaviour
{
    public Action OnClick;
    
    public void OnMouseDown()
    {
        Debug.Log($"CLICK {name}");
        OnClick?.Invoke();
    }
}