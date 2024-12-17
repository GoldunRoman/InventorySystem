using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _title;

    private bool _isMouseOver;
    public ItemType ItemType { get; private set; }

    public void Initialize(Sprite icon, string title, ItemType itemType)
    {
        _icon.sprite = icon;
        _title.text = title;
        ItemType = itemType;
    }

    private void OnDisable() => _isMouseOver = false;

    public bool IsMouseOver() => _isMouseOver;

    public void OnPointerEnter(PointerEventData eventData) => _isMouseOver = true;

    public void OnPointerExit(PointerEventData eventData) => _isMouseOver = false;
}