using UnityEngine;
using KittyTerror.Events;

public class Toolbox : MonoBehaviour, IInteractable
{
    [SerializeField] private string requiredItem = "Llave";
    [SerializeField] private string rewardItem = "Hacha";
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool _opened;

    public string GetInteractText()
    {
        if (_opened) return "";
        return "Abrir mueble (requiere Llave)";
    }

    public void Interact()
    {
        if (_opened) return;

        Inventory inv = FindObjectOfType<Inventory>();
        if (inv == null || !inv.HasItem(requiredItem))
        {
            Debug.Log($"[Toolbox] Necesitas {requiredItem} para abrir el mueble");
            return;
        }

        inv.RemoveItem(requiredItem);
        inv.AddItem(rewardItem);
        _opened = true;

        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("item_pickup"));
        Debug.Log($"[Toolbox] {requiredItem} abre el mueble — Obtuviste {rewardItem}");

        if (openedSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = openedSprite;
    }
}
