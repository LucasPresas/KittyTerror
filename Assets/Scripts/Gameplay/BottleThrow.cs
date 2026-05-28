using UnityEngine;
using UnityEngine.Events;
using KittyTerror.Gameplay;

public class BottleThrow : MonoBehaviour, IItemAction
{
    [SerializeField] private GameObject bottleInHand;

    public UnityEvent OnThrowAnimation;

    private Inventory _inventory;

    private void Start()
    {
        _inventory = GetComponent<Inventory>();
        if (_inventory == null)
            _inventory = FindObjectOfType<Inventory>();

        if (bottleInHand != null)
            bottleInHand.SetActive(false);
    }

    public void OnItemUsed(string itemId)
    {
        if (itemId != "Botella") return;
        Throw();
    }

    private void Throw()
    {
        if (_inventory == null || !_inventory.HasItem("Botella"))
            return;

        _inventory.RemoveItem("Botella");

        ItemEquip equip = GetComponent<ItemEquip>();
        if (equip != null)
            equip.Unequip();

        if (bottleInHand != null)
            bottleInHand.SetActive(false);

        OnThrowAnimation?.Invoke();

        CatStateMachineController cat = FindObjectOfType<CatStateMachineController>();
        if (cat != null)
            cat.Flee();
    }
}
