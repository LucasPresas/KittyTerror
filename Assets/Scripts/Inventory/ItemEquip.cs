using UnityEngine;

public class ItemEquip : MonoBehaviour
{
    [System.Serializable]
    public struct ItemSlot
    {
        public string itemId;
        public GameObject itemObject;
    }

    [SerializeField] private ItemSlot[] items;

    private string _currentItem;

    private void Start()
    {
        Inventory inv = FindObjectOfType<Inventory>();
        if (inv != null)
        {
            inv.OnItemRemoved.AddListener(OnItemRemoved);
            inv.OnItemSelected.AddListener(EquipItem);
        }
    }

    private void OnItemRemoved(string itemId)
    {
        if (_currentItem == itemId)
            EquipItem("");
    }

    public void EquipItem(string itemId)
    {
        _currentItem = itemId;

        foreach (var slot in items)
        {
            if (slot.itemObject != null)
                slot.itemObject.SetActive(slot.itemId == itemId);
        }
    }
}
