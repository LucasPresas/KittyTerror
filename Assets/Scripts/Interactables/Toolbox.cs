using UnityEngine;

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
        if (inv == null || !inv.HasItem(requiredItem)) return;

        inv.RemoveItem(requiredItem);
        inv.AddItem(rewardItem);
        _opened = true;

        if (openedSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = openedSprite;
    }
}
