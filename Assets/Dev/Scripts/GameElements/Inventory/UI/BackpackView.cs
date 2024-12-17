using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class BackpackView : MonoBehaviour
{
    public Action<ItemType> ItemRemoved;

    [SerializeField] private GameObject _ui;
    [SerializeField] private LayerMask _backpackLayer;
    [SerializeField] private ItemView _prefab;
    [SerializeField] private Transform _itemContainer;

    private BackpackDataProvider _dataProvider;
    private InputActionAsset _inputActions;
    private InputAction _mousePressAction;
    private Camera _mainCamera;

    private bool _isMouseOverBackpack = false;

    #region Zenject Constructor
    [Inject]
    public void Construct(BackpackDataProvider dataProvider, InputActionAsset inputActions)
    {
        _dataProvider = dataProvider;
        _inputActions = inputActions;
    }
    #endregion

    #region Initialization
    public void Initialize()
    {
        _mainCamera = Camera.main;
        _ui.SetActive(false);
    }
    #endregion

    #region MonoBehaviour Methods
    private void OnEnable()
    {
        _mousePressAction = _inputActions["Press"];

        _mousePressAction.performed += OnMouseClick;
        _mousePressAction.canceled += OnMouseRelease;
    }

    private void OnDisable()
    {
        _mousePressAction.performed -= OnMouseClick;
        _mousePressAction.canceled -= OnMouseRelease;
    }

    private void Update()
    {
        CheckMouseOverBackpack();
    }
    #endregion

    #region View Methods
    private void PopulateBackpackUI()
    {
        ClearBackpackUI();

        List<ItemType> itemsInBackpack = _dataProvider.GetItemsInBackpack();

        foreach (ItemType itemType in itemsInBackpack)
        {
            ItemView itemView = Instantiate(_prefab, _itemContainer);
            Sprite icon = _dataProvider.GetItemIcon(itemType);
            string name = _dataProvider.GetItemName(itemType);

            itemView.Initialize(icon, name, itemType);
        }
    }

    private void ClearBackpackUI()
    {
        foreach (Transform child in _itemContainer)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion

    #region Callbacks
    private void OnMouseClick(InputAction.CallbackContext context)
    {
        if (_isMouseOverBackpack)
        {
            _ui.SetActive(true);
            PopulateBackpackUI();
        }
    }

    private void OnMouseRelease(InputAction.CallbackContext context)
    {
        CheckItemViewInteraction();
        _ui.SetActive(false);
    }
    #endregion

    #region Utility Methods
    private void CheckMouseOverBackpack()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _backpackLayer))
        {
            _isMouseOverBackpack = true;
        }
        else
        {
            _isMouseOverBackpack = false;
        }
    }

    private void CheckItemViewInteraction()
    {
        foreach (Transform child in _itemContainer)
        {
            ItemView itemView = child.GetComponent<ItemView>();
            if (itemView != null && itemView.IsMouseOver())
            {
                ItemRemoved?.Invoke(itemView.ItemType);
                return;
            }
        }
    }
    #endregion
}