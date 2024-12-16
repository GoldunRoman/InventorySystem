using UnityEngine;

public class InventoryItem : MonoBehaviour, IDragable
{
    public ItemType ItemType { get; protected set; }

    public void OnDragEnd()
    {
        throw new System.NotImplementedException();
    }

    public void OnDragStart()
    {
        throw new System.NotImplementedException();
    }
}
