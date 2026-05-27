using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<string> _items = new List<string>();

    public bool HasItem(string id) => _items.Contains(id);

    public void AddItem(string id)
    {
        if (!_items.Contains(id))
            _items.Add(id);
    }

    public void RemoveItem(string id)
    {
        _items.Remove(id);
    }
}
