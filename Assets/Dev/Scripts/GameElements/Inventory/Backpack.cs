using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class Backpack : MonoBehaviour
{
    [SerializeField] private float _pickupRadius = 2.0f;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private List<BackpackSlot> _itemSlots;

    public bool TryCollectItem(IDragable dragable)
    {
        float distance = Vector3.Distance(transform.position, dragable.Transform.position);

        if (distance <= _pickupRadius)
        {
            CollectItem(dragable);
            return true;
        }

        return false;
    }

    private void CollectItem(IDragable dragable)
    {
        BackpackSlot availableSlot = GetFreeSlot(dragable);

        if (availableSlot != null)
        {
            InventoryItem item = dragable as InventoryItem;
            item.IsInBackpack = true;
            availableSlot.IsFree = false;

            dragable.Transform.DOLocalMove(Vector3.zero, _moveDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    dragable.OnDragEnd();
                });
        }
        else
        {
            Debug.LogWarning("No free slots!");
        }
    }

    private BackpackSlot GetFreeSlot(IDragable dragable)
    {
        InventoryItem item = dragable as InventoryItem;
        return _itemSlots.FirstOrDefault(slt => slt.ItemType == item.ItemType && slt.IsFree);
    }

    public void ReleaseItemSlot(ItemType itemType)
    {
        _itemSlots.FirstOrDefault(slt => slt.ItemType == itemType).IsFree = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _pickupRadius);
    }

    [System.Serializable]
    private class BackpackSlot
    {
        [field: SerializeField] public ItemType ItemType { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public bool IsFree { get; set; }
    }
}
