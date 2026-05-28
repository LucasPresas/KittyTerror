using UnityEngine;
using UnityEngine.InputSystem;
using KittyTerror.Gameplay;

public class BottleThrow : MonoBehaviour
{
    [SerializeField] private GameObject bottleInHand;

    private Inventory _inventory;
    private bool _equipped;

    private void Start()
    {
        _inventory = GetComponent<Inventory>();
        if (bottleInHand != null)
            bottleInHand.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            if (_equipped)
                Throw();
            else
                Equip();
        }
    }

    private void Equip()
    {
        if (_inventory == null || !_inventory.HasItem("Botella"))
        {
            Debug.Log("[Bottle] No tenés Botella en el inventario");
            return;
        }

        _equipped = true;
        if (bottleInHand != null)
            bottleInHand.SetActive(true);

        Debug.Log("[Bottle] Botella en mano! Presioná G para lanzar");
    }

    private void Throw()
    {
        _equipped = false;
        if (bottleInHand != null)
            bottleInHand.SetActive(false);

        _inventory.RemoveItem("Botella");
        Debug.Log($"[Bottle] Botella lanzada! Restan: {_inventory.CountItem("Botella")}");

        CatStateMachineController cat = FindObjectOfType<CatStateMachineController>();
        if (cat != null)
        {
            cat.Flee();
            Debug.Log("[Bottle] Gato ahuyentado!");
        }
    }
}
