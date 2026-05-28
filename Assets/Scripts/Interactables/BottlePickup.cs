using UnityEngine;

public class BottlePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactText = "Agarrar botella";

    public string GetInteractText() => interactText;

    public void Interact()
    {
        Inventory inv = FindObjectOfType<Inventory>();
        if (inv != null)
        {
            inv.AddItem("Botella");
            Debug.Log("[BottlePickup] Agarraste Botella");
            Destroy(gameObject);
        }
    }
}
