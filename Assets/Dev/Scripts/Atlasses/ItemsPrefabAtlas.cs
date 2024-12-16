using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsPrefabAtlas", menuName = "Custom/Atlasses/ItemsPrefabAtlas")]
public class ItemsPrefabAtlas : ScriptableObject
{
    [SerializeField] private List<ItemData> _itemsDatas;

    public InventoryItem GetItem(ItemType itemType) => _itemsDatas.FirstOrDefault(itm => itm.Type == itemType).Prefab;

    [System.Serializable]
    private class ItemData 
    {
        [field: SerializeField] public InventoryItem Prefab { get; private set; }
        [field: SerializeField] public InventoryItemConfig Config { get; private set; }
        public ItemType Type { get { return Config.ItemType; } }
    }
}