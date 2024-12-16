using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemConfig", menuName = "Custom/Config/InventoryItemConfig")]
public class InventoryItemConfig : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public float Weight { get; private set; }
    [field: SerializeField] public ItemType ItemType { get; private set; }
}