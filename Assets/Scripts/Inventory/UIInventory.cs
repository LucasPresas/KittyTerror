using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    public static UIInventory Instance;

    public Image[] slots;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < Inventory.Instance.items.Count)
            {
                slots[i].sprite = Inventory.Instance.items[i].icon;
                slots[i].enabled = true;
            }
            else
            {
                slots[i].enabled = false;
            }
        }
    }
}