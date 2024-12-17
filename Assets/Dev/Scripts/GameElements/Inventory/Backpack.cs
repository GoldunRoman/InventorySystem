using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using Zenject;
using UnityEngine.Events;
using System;

public class Backpack : MonoBehaviour
{
    private UnityEvent<int> _itemAddedToBackpack = new UnityEvent<int>();
    private UnityEvent<int> _itemRemovedFromBackpack = new UnityEvent<int>();

    [SerializeField] private float _pickupRadius = 2.0f;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private float _liftHeight = 2f;
    [SerializeField] private List<BackpackSlot> _itemSlots;

    private InventoryServerCommunicator _serverCommunicator;

    #region Zenject Constructor
    [Inject]
    public void Construct(InventoryServerCommunicator serverCommunicator)
    {
        _serverCommunicator = serverCommunicator;
    }
    #endregion

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        _itemAddedToBackpack.AddListener(SendItemAddedToServer);
        _itemRemovedFromBackpack.AddListener(SendItemRemovedToServer);
    }

    private void OnDisable()
    {
        _itemAddedToBackpack.RemoveListener(SendItemAddedToServer);
        _itemRemovedFromBackpack.RemoveListener(SendItemRemovedToServer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _pickupRadius);
    }
    #endregion

    #region Collect Methods
    public bool TryCollectItem(IDragable dragable)
    {
        float distance = Vector3.Distance(transform.position, dragable.Transform.position);

        if (distance <= _pickupRadius)
        {
            CollectItem(dragable as InventoryItem);
            return true;
        }

        return false;
    }

    private void CollectItem(InventoryItem inventoryItem)
    {
        BackpackSlot availableSlot = GetFreeSlot(inventoryItem);

        if (availableSlot != null)
        {
            inventoryItem.IsInBackpack = true;
            availableSlot.IsFree = false;

            Vector3 startPos = inventoryItem.Transform.position;
            Vector3 targetPos = availableSlot.Transform.position;

            Vector3 arcPoint = (startPos + targetPos) / 2 + Vector3.up * _liftHeight;

            Vector3[] path = new Vector3[] { startPos, arcPoint, targetPos };

            inventoryItem.Transform.DOPath(path, _moveDuration, PathType.CatmullRom)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    inventoryItem.OnDragEnd();
                    _itemAddedToBackpack.Invoke(inventoryItem.ID);
                });
        }
        else
        {
            Debug.LogWarning("No free slots!");
        }
    }

    public void ReleaseItemSlot(InventoryItem item)
    {
        _itemSlots.FirstOrDefault(slt => slt.ItemType == item.ItemType).IsFree = true;
        _itemRemovedFromBackpack.Invoke(item.ID);
    }
    #endregion

    #region Utility Methods
    private BackpackSlot GetFreeSlot(InventoryItem item)
    {
        return _itemSlots.FirstOrDefault(slt => slt.ItemType == item.ItemType && slt.IsFree);
    }
    #endregion

    #region Callbacks
    private async void SendItemAddedToServer(int itemId)
    {
        try
        {
            await _serverCommunicator.SendItemStatusAsync(itemId, "added");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending item added status: {ex.Message}");
        }
    }

    private async void SendItemRemovedToServer(int itemId)
    {
        try
        {
            await _serverCommunicator.SendItemStatusAsync(itemId, "removed");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error sending item removed status: {ex.Message}");
        }
    }
    #endregion

    [System.Serializable]
    private class BackpackSlot
    {
        [field: SerializeField] public ItemType ItemType { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public bool IsFree { get; set; }
    }
}
