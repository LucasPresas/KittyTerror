using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string itemId;
    [SerializeField] private string interactText = "Agarrar";

    public string GetInteractText() => interactText;

    public void Interact()
    {
        Inventory inv = FindObjectOfType<Inventory>();
        if (inv != null)
        {
            inv.AddItem(itemId);
            Destroy(gameObject);
        }
    }
}
