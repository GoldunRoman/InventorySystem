using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class DragDropHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _liftHeight = 1.0f;

    private Backpack _backpack;

    private IDragable _draggingItem;
    private Vector3 _offsetFromGround;
    private Camera _mainCamera;

    #region Zenject Constructor
    [Inject]
    public void Construct(Backpack backpack)
    {
        _backpack = backpack;
    }
    #endregion

    #region Initialization
    public void Initialize()
    {
        _mainCamera = Camera.main;
    }
    #endregion

    #region Dragging Methods
    public void StartDragging(IDragable item)
    {
        if (_draggingItem != null) return;

        _draggingItem = item;

        if (TryGetMouseGroundPosition(out Vector3 groundPosition))
        {
            _offsetFromGround = _draggingItem.Transform.position - groundPosition;
        }

        Vector3 liftedPosition = _draggingItem.Transform.position;
        liftedPosition.y += _liftHeight;
        _draggingItem.Transform.position = liftedPosition;

        _draggingItem.OnDragStart();
    }

    public void EndDragging()
    {
        if (_draggingItem == null)
            return;

        if (_draggingItem is InventoryItem item)
        {
            if (item.IsInBackpack)
            {
                _backpack.ReleaseItemSlot(item);
                item.IsInBackpack = false;
            }
            else
            {
                if (_backpack.TryCollectItem(_draggingItem))
                {
                    item.IsInBackpack = true;
                    _draggingItem = null;
                    return;
                }
            }
        }

        if (TryGetMouseGroundPosition(out Vector3 groundPosition))
        {
            _draggingItem.Transform.position = groundPosition;
        }

        _draggingItem.OnDragEnd();
        _draggingItem = null;
    }

    public void DragItem(Vector2 mousePosition)
    {
        if (_draggingItem == null) return;

        if (TryGetMouseGroundPosition(out Vector3 groundPosition))
        {
            Vector3 targetPosition = groundPosition + _offsetFromGround;
            targetPosition.y = _draggingItem.Transform.position.y;
            _draggingItem.Transform.position = targetPosition;
        }
    }
    #endregion

    #region Utility Methods
    private bool TryGetMouseGroundPosition(out Vector3 groundPosition)
    {
        groundPosition = Vector3.zero;

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
        {
            groundPosition = hit.point;
            return true;
        }

        return false;
    }
    #endregion
}
