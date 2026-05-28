using UnityEngine;

public class Drawer : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemId = "Llave";
    [SerializeField] private string interactText = "Abrir cajón";
    [SerializeField] private Sprite openedSprite;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool _opened;

    public string GetInteractText() => _opened ? "" : interactText;

    public void Interact()
    {
        if (_opened) return;

        _opened = true;

        if (openedSprite != null && spriteRenderer != null)
            spriteRenderer.sprite = openedSprite;

        Inventory inv = FindObjectOfType<Inventory>();
        if (inv != null)
        {
            inv.AddItem(itemId);
            Debug.Log($"[Drawer] Obtuviste {itemId}");
        }
    }
}
