using System;
using UnityEngine;
using Zenject;

public class ItemsCreator : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private float _spawnRadius;

    private ItemsFactory _itemsFactory;

    [Inject]
    public void Construct(ItemsFactory itemsFactory)
    {
        _itemsFactory = itemsFactory;
    }

    public void Initialize()
    {
        SpawnItems();
    }

    private void SpawnItems()
    {
        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            Vector3 randomPosition = GetRandomPositionInRadius();

            InventoryItem item = _itemsFactory.Create(itemType);
            item.transform.position = randomPosition;
        }
    }

    private Vector3 GetRandomPositionInRadius()
    {
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * _spawnRadius;
        return _spawnPoint.position + new Vector3(randomPoint.x, 0, randomPoint.y);
    }
}
