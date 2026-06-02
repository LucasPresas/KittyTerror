using UnityEngine;
using KittyTerror.Events;

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
            EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("item_pickup"));
            Debug.Log($"[ItemPickup] Agarraste {itemId}");
            Destroy(gameObject);
        }
    }
}
