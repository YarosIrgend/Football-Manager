using UnityEngine;

public class PropertyClickable : MonoBehaviour
{
    private Player player;
    private PropertyManager manager;

    public void Init(Player p, PropertyManager m)
    {
        player = p;
        manager = m;
    }

    private void OnMouseDown()
    {
        manager.ShowPropertyPanel(player);
        Debug.Log("Property panel selected");
    }
}