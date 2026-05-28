using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class ItemEvent : UnityEvent<string> { }

    public ItemEvent OnItemAdded;
    public ItemEvent OnItemRemoved;
    public ItemEvent OnItemSelected;

    private List<string> _items = new List<string>();

    public bool HasItem(string id) => _items.Contains(id);

    public void AddItem(string id)
    {
        if (!_items.Contains(id))
        {
            _items.Add(id);
            Debug.Log($"[Inventory] Agregado: {id}");
            OnItemAdded?.Invoke(id);
        }
    }

    public void RemoveItem(string id)
    {
        if (_items.Remove(id))
        {
            Debug.Log($"[Inventory] Eliminado: {id}");
            OnItemRemoved?.Invoke(id);
        }
    }

    public void SelectItem(string id)
    {
        OnItemSelected?.Invoke(id);
    }
}
