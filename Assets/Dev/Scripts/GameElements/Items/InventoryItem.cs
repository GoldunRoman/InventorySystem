using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InventoryItem : MonoBehaviour, IDragable
{
    private Rigidbody _rigidbody;
    private InventoryItemConfig _config;

    public ItemType ItemType { get { return _config.ItemType; } }
    public Transform Transform { get { return transform; } }
    public int ID { get {  return _config.ID; } } 
    public bool IsInBackpack { get; set; }

    public void Initialize(InventoryItemConfig config)
    {
        _config = config;
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void OnDragEnd()
    {
        _rigidbody.isKinematic = false;
        transform.rotation = Quaternion.identity;
    }

    public void OnDragStart()
    {
        _rigidbody.isKinematic = true;
    }
}
