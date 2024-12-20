using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private ItemsPrefabAtlas _itemsPrefabAtlas;
    [SerializeField] private ItemsCreator _itemsCreator;
    [SerializeField] private DragDropHandler _dragDropHandler;
    [SerializeField] private InputActionAsset _inputActions;
    [SerializeField] private Backpack _backpack;
    [SerializeField] private ItemsIconAtlas _itemsIconAtlas;
    [SerializeField] private BackpackView _backpackView;

    public override void InstallBindings()
    {
        #region General Bindings
        Container.Bind<ItemsPrefabAtlas>().FromInstance(_itemsPrefabAtlas).AsSingle();
        Container.Bind<ItemsCreator>().FromInstance(_itemsCreator).AsSingle();
        Container.Bind<DragDropHandler>().FromInstance(_dragDropHandler).AsSingle();
        Container.Bind<InputActionAsset>().FromInstance(_inputActions).AsSingle();
        Container.Bind<Backpack>().FromInstance(_backpack).AsSingle();
        Container.Bind<InventoryServerCommunicator>().AsSingle();
        Container.Bind<ItemsIconAtlas>().FromInstance(_itemsIconAtlas).AsSingle();
        Container.Bind<BackpackDataProvider>().AsSingle();
        Container.Bind<BackpackView>().FromInstance(_backpackView).AsSingle();
        #endregion

        #region Factory Bindings
        Container.BindFactory<ItemType, InventoryItem, ItemsFactory>()
            .To<InventoryItem>()
            .FromFactory<ItemsFactory>();
        #endregion
    }
}