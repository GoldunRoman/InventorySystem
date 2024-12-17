using UnityEngine;

public interface IDragable
{
    public Transform Transform { get; } 
    public void OnDragStart();
    public void OnDragEnd();
}