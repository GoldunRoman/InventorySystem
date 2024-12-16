using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private ItemsPrefabAtlas _itemsPrefabAtlas;
    [SerializeField] private ItemsCreator _itemsCreator;

    public override void InstallBindings()
    {
        #region General Bindings
        Container.Bind<ItemsPrefabAtlas>().FromInstance(_itemsPrefabAtlas).AsSingle();
        Container.Bind<ItemsCreator>().FromInstance(_itemsCreator).AsSingle();
        #endregion

        #region Factory Bindings
        Container.BindFactory<ItemType, InventoryItem, ItemsFactory>()
            .To<InventoryItem>()
            .FromFactory<ItemsFactory>();
        #endregion
    }
}