using UnityEngine;
using UnityEngine.InputSystem;

public class DragDropHandler : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _liftHeight = 1.0f;

    private IDragable _draggingItem;
    private Vector3 _offsetFromGround;
    private Camera _mainCamera;

    public void Initialize()
    {
        _mainCamera = Camera.main;
    }

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
        if (_draggingItem == null) return;

        if (TryGetMouseGroundPosition(out Vector3 groundPosition))
        {
            _draggingItem.Transform.position = groundPosition + Vector3.up * 0.01f;
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
}
