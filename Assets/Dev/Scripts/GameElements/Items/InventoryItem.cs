using UnityEngine;

public class InventoryItem : MonoBehaviour, IDragable
{
    private InventoryItemConfig _config;

    public ItemType ItemType { get { return _config.ItemType; } }
    public Transform Transform { get { return transform; } }

    public void Initialize(InventoryItemConfig config)
    {
        _config = config;
    }

    public void OnDragEnd()
    {
    }

    public void OnDragStart()
    {
    }
}
