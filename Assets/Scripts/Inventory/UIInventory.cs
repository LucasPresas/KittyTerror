using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    [System.Serializable]
    public class SlotUI
    {
        public string itemId;
        public Image icon;
    }

    public static UIInventory Instance;

    public SlotUI[] slots;

    private Inventory _inventory;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _inventory = FindObjectOfType<Inventory>();
        if (_inventory == null)
        {
            Debug.LogError("[UIInventory] No se encontró Inventory en la escena.");
            return;
        }

        _inventory.OnItemAdded.AddListener(UpdateUI);
        _inventory.OnItemRemoved.AddListener(UpdateUI);

        UpdateUI("");
    }

    private void UpdateUI(string itemId)
    {
        if (_inventory == null) return;

        foreach (var slot in slots)
        {
            if (slot == null || slot.icon == null) continue;

            bool hasItem = _inventory.HasItem(slot.itemId);
            slot.icon.gameObject.SetActive(hasItem);
        }
    }

    [ContextMenu("Forzar Actualización UI")]
    public void ForceRefresh()
    {
        UpdateUI("");
    }
}
