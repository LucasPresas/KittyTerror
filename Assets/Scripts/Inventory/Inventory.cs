using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public List<ItemData> items = new List<ItemData>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddItem(ItemData item)
    {
        items.Add(item);

        UIInventory.Instance.UpdateUI();
    }
}