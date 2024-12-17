using UnityEngine;
using Zenject;

public class EntryPoint : MonoBehaviour
{
    private ItemsCreator _itemsCreator;
    private DragDropHandler _dragDropHandler;
    private BackpackView _backpackView;

    [Inject]
    public void Construct(ItemsCreator itemsCreator, DragDropHandler dragDropHandler, BackpackView backpackView)
    {
        _itemsCreator = itemsCreator;
        _dragDropHandler = dragDropHandler;
        _backpackView = backpackView;
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _itemsCreator.Initialize();
        _dragDropHandler.Initialize();
        _backpackView.Initialize();
    }
}
