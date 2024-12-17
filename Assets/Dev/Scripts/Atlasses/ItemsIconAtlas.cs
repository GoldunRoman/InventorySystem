using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemsIconAtlas", menuName = "Custom/Atlasses/ItemsIconAtlas")]
public class ItemsIconAtlas : ScriptableObject
{
    [SerializeField] private List<IconData> _itemsIcons;

    public Sprite GetItemData(ItemType itemType) => _itemsIcons.FirstOrDefault(d => d.ItemType == itemType).Icon;

    [System.Serializable]
    public class IconData
    {
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public ItemType ItemType { get; private set; }
    }
}
