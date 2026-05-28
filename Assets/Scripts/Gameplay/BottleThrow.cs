using UnityEngine;
using UnityEngine.Events;
using KittyTerror.Gameplay;

public class BottleThrow : MonoBehaviour, IItemAction
{
    [SerializeField] private GameObject bottleInHand;
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float destroyDelay = 3f;

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

        OnThrowAnimation?.Invoke();

        LaunchBottle();

        CatStateMachineController cat = FindObjectOfType<CatStateMachineController>();
        if (cat != null)
            cat.Flee();
    }

    private void LaunchBottle()
    {
        if (bottleInHand == null) return;

        bottleInHand.SetActive(true);
        bottleInHand.transform.SetParent(null);

        Rigidbody rb = bottleInHand.GetComponent<Rigidbody>();
        if (rb == null)
            rb = bottleInHand.AddComponent<Rigidbody>();

        Camera cam = GetComponentInChildren<Camera>();
        Vector3 direction = cam != null ? cam.transform.forward : transform.forward;

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(direction * throwForce, ForceMode.VelocityChange);

        Destroy(bottleInHand, destroyDelay);
    }
}
