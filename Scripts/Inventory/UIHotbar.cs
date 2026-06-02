using UnityEngine;
using UnityEngine.UI;

public class UIHotbar : MonoBehaviour
{
    [System.Serializable]
    public class SlotUI
    {
        public string itemId;
        public Image icon;
    }

    [Header("Slots de la Hotbar")]
    public SlotUI[] slots;

    private Inventory inventory;

    private void Start()
    {
        Debug.Log("[UIHotbar] Iniciando...");

        inventory = FindObjectOfType<Inventory>();

        if (inventory == null)
        {
            Debug.LogError("[UIHotbar] No se encontró ningún Inventory en la escena.");
            return;
        }

        Debug.Log("[UIHotbar] Inventory encontrado en: " + inventory.gameObject.name);

        inventory.OnItemAdded.AddListener(UpdateHotbar);
        inventory.OnItemRemoved.AddListener(UpdateHotbar);

        Debug.Log("[UIHotbar] Eventos registrados correctamente.");

        UpdateHotbar("");
    }

    private void UpdateHotbar(string itemId)
    {
        Debug.Log("[UIHotbar] Actualizando HUD. Evento recibido: " + itemId);

        if (inventory == null)
        {
            Debug.LogError("[UIHotbar] Inventory es NULL.");
            return;
        }

        foreach (var slot in slots)
        {
            if (slot == null)
            {
                Debug.LogWarning("[UIHotbar] Slot NULL.");
                continue;
            }

            if (slot.icon == null)
            {
                Debug.LogWarning("[UIHotbar] Icono no asignado para: " + slot.itemId);
                continue;
            }

            bool hasItem = inventory.HasItem(slot.itemId);

            Debug.Log(
                $"[UIHotbar] Item: {slot.itemId} | Tiene: {hasItem}"
            );

            // Mostrar u ocultar el GameObject completo
            slot.icon.gameObject.SetActive(hasItem);
        }
    }

    // Botón de prueba para actualizar manualmente
    [ContextMenu("Forzar Actualización HUD")]
    public void ForceRefresh()
    {
        UpdateHotbar("ManualRefresh");
    }
}