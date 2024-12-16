public class Queen : InventoryItem
{
    private void OnValidate()
    {
        ItemType = ItemType.Queen;
    }
}