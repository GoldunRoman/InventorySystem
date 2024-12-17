using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class DragDropInput : MonoBehaviour
{
    private DragDropHandler _dragDropHandler;
    private InputActionAsset _actionMap;

    private InputAction _mousePressAction;
    private InputAction _mousePositionAction;

    private IDragable _currentTarget;

    [Inject]
    public void Construct(DragDropHandler dragDropHandler, InputActionAsset inputActions)
    {
        _dragDropHandler = dragDropHandler;
        _actionMap = inputActions;
    }

    private void OnEnable()
    {
        _mousePressAction = _actionMap["Press"];
        _mousePositionAction = _actionMap["ScreenPosition"];

        _mousePressAction.performed += OnMousePress;
        _mousePressAction.canceled += OnMouseRelease;

        _actionMap.Enable();
    }

    private void OnDisable()
    {
        _mousePressAction.performed -= OnMousePress;
        _mousePressAction.canceled -= OnMouseRelease;

        _actionMap.Disable();
    }

    private void Update()
    {
        DetectItemUnderCursor();

        if (_mousePressAction.IsPressed() && _dragDropHandler != null)
        {
            Vector2 mousePosition = _mousePositionAction.ReadValue<Vector2>();
            _dragDropHandler.DragItem(mousePosition);
        }
    }


    private void DetectItemUnderCursor()
    {
        Vector2 mousePosition = _mousePositionAction.ReadValue<Vector2>();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _currentTarget = hit.collider.GetComponent<IDragable>();
        }
        else
        {
            _currentTarget = null;
        }
    }

    private void OnMousePress(InputAction.CallbackContext context)
    {
        if (_currentTarget != null)
        {
            _dragDropHandler.StartDragging(_currentTarget);
        }
    }

    private void OnMouseRelease(InputAction.CallbackContext context)
    {
        _dragDropHandler.EndDragging();
    }
}
