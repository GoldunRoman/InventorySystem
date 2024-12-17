using System.Collections.Generic;
using UnityEngine;

public class BackpackDataProvider
{
    private readonly Backpack _backpack;
    private readonly ItemsIconAtlas _iconAtlas;

    public BackpackDataProvider(Backpack backpack, ItemsIconAtlas iconAtlas)
    {
        _backpack = backpack;
        _iconAtlas = iconAtlas;
    }

    public List<ItemType> GetItemsInBackpack()
    {
        List<ItemType> items = new List<ItemType>();

        foreach (var slot in _backpack.ItemSlots)
        {
            if (!slot.IsFree)
            {
                items.Add(slot.ItemType);
            }
        }

        return items;
    }

    public Sprite GetItemIcon(ItemType itemType)
    {
        return _iconAtlas.GetItemData(itemType);
    }

    public string GetItemName(ItemType itemType)
    {
        return itemType.ToString();
    }
}