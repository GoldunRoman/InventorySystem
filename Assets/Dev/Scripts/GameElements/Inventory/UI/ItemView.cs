using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _title;

    private ItemType _itemType;

    public void Initialize(Sprite icon, string title, ItemType itemType)
    {
        _icon.sprite = icon;
        _title.text = title;
        _itemType = itemType;
    }
}