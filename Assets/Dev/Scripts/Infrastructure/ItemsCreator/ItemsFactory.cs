using UnityEngine;
using Zenject;

public class ItemsFactory : PlaceholderFactory<ItemType, InventoryItem>
{
    private ItemsPrefabAtlas _atlas;

    [Inject]
    public void Construct(ItemsPrefabAtlas atlas)
    {
        _atlas = atlas;
    }

    public override InventoryItem Create(ItemType itemType)
    {
        ItemsPrefabAtlas.ItemData itemData = _atlas.GetItemData(itemType);

        if (itemData == null)
        {
            throw new System.Exception($"ItemData for ItemType {itemType} not found in ItemsPrefabAtlas.");
        }

        InventoryItem item = GameObject.Instantiate(itemData.Prefab);
        item.Initialize(itemData.Config);

        return item;
    }
}
