using UnityEngine;
using Zenject;

public class EntryPoint : MonoBehaviour
{
    private ItemsCreator _itemsCreator;
    private DragDropHandler _dragDropHandler;

    [Inject]
    public void Construct(ItemsCreator itemsCreator, DragDropHandler dragDropHandler)
    {
        _itemsCreator = itemsCreator;
        _dragDropHandler = dragDropHandler;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _itemsCreator.Initialize();
        _dragDropHandler.Initialize();
    }
}
