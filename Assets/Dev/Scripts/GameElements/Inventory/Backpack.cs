using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class Backpack : MonoBehaviour
{
    private UnityEvent<int> _itemAddedToBackpack = new UnityEvent<int>();
    private UnityEvent<int> _itemRemovedFromBackpack = new UnityEvent<int>();

    [Header("Pickup Settings")]
    [SerializeField] private float _pickupRadius = 11.0f;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private float _liftHeight = 2f;

    [Header("Drop Settings")]
    [SerializeField] private float _dropDistance = 9.0f;
    [SerializeField] private float _dropOffsetY = 2.0f;

    private InventoryServerCommunicator _serverCommunicator;
    private BackpackView _backpackView;
    private List<InventoryItem> _currentItems = new List<InventoryItem>();

    [field: SerializeField] public List<BackpackSlot> ItemSlots { get; private set; }

    #region Zenject Constructor
    [Inject]
    public void Construct(InventoryServerCommunicator serverCommunicator, BackpackView backpackView)
    {
        _serverCommunicator = serverCommunicator;
        _backpackView = backpackView;
    }
    #endregion

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        _itemAddedToBackpack.AddListener(SendItemAddedToServer);
        _itemRemovedFromBackpack.AddListener(SendItemRemovedToServer);
        _backpackView.ItemRemoved += ReleaseItemSlotWithAnimation;
    }

    private void OnDisable()
    {
        _itemAddedToBackpack.RemoveListener(SendItemAddedToServer);
        _itemRemovedFromBackpack.RemoveListener(SendItemRemovedToServer);
        _backpackView.ItemRemoved -= ReleaseItemSlotWithAnimation;
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

    public void ReleaseItemSlot(InventoryItem item)
    {
        if (_currentItems.Contains(item))
        {
            ItemSlots.FirstOrDefault(slt => slt.ItemType == item.ItemType).IsFree = true;
            _currentItems.Remove(item);

            _itemRemovedFromBackpack?.Invoke(item.ID);
        }
        else
        {
            Debug.LogError($"List does not contain item");
        }
    }

    public void ReleaseItemSlotWithAnimation(ItemType itemType)
    {
        BackpackSlot itemSlot = ItemSlots.FirstOrDefault(slot => slot.ItemType == itemType && !slot.IsFree);
        if (itemSlot == null)
        {
            Debug.LogWarning($"[Backpack] No slot found for type {itemType} or it's already free.");
            return;
        }

        InventoryItem inventoryItem = _currentItems.FirstOrDefault(inv => inv.ItemType == itemType && inv.IsInBackpack);

        if (inventoryItem == null)
        {
            Debug.LogWarning($"[Backpack] No inventory item of type {itemType} found in backpack.");
            return;
        }

        if (DOTween.IsTweening(inventoryItem.Transform))
        {
            Debug.LogWarning($"[Backpack] Item {itemType} is already being animated.");
            return;
        }

        Vector3 startPos = inventoryItem.Transform.position;
        Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        Vector3 targetPos = transform.position + randomDirection * _dropDistance;
        Vector3 arcPoint = (startPos + targetPos) / 2 + Vector3.up * _dropOffsetY;

        Vector3[] path = new Vector3[] { startPos, arcPoint, targetPos };

        inventoryItem.Transform.DOPath(path, _moveDuration, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                inventoryItem.IsInBackpack = false;
                itemSlot.IsFree = true;

                if (_currentItems.Contains(inventoryItem))
                {
                    _currentItems.Remove(inventoryItem);
                    _itemRemovedFromBackpack?.Invoke(inventoryItem.ID);
                }
                else
                {
                    Debug.LogWarning($"[Backpack] Item {inventoryItem.ItemType} already removed from backpack.");
                }
            });
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

                    if (!_currentItems.Contains(inventoryItem))
                    {
                        _currentItems.Add(inventoryItem);
                    }
                    else
                    {
                        Debug.LogWarning("Item is already in the backpack!");
                    }

                    _itemAddedToBackpack?.Invoke(inventoryItem.ID);
                });
        }
        else
        {
            Debug.LogWarning("No free slots!");
        }
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

    #region Utility Methods
    private BackpackSlot GetFreeSlot(InventoryItem item)
    {
        return ItemSlots.FirstOrDefault(slt => slt.ItemType == item.ItemType && slt.IsFree);
    }
    #endregion

    [System.Serializable]
    public class BackpackSlot
    {
        [field: SerializeField] public ItemType ItemType { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public bool IsFree { get; set; }
    }
}
