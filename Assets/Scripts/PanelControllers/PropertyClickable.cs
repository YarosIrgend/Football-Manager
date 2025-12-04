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

    public void OnMouseDown()
    {
        manager.ShowPropertyPanel(player);
    }
}