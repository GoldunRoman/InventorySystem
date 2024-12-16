using UnityEngine;
using Zenject;

public class EntryPoint : MonoBehaviour
{
    private ItemsCreator _itemsCreator;

    [Inject]
    public void Construct(ItemsCreator itemsCreator)
    {
        _itemsCreator = itemsCreator;
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _itemsCreator.Initialize();
    }
}
