using UnityEngine;

public class InventoryItem : MonoBehaviour, IDragable
{
    private InventoryItemConfig _config;

    public ItemType ItemType { get { return _config.ItemType; } }

    public void Initialize(InventoryItemConfig config)
    {
        _config = config;
    }

    public void OnDragEnd()
    {
        throw new System.NotImplementedException();
    }

    public void OnDragStart()
    {
        throw new System.NotImplementedException();
    }
}
