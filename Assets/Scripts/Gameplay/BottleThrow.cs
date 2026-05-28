using UnityEngine;
using UnityEngine.InputSystem;
using KittyTerror.Gameplay;

public class BottleThrow : MonoBehaviour
{
    private Inventory _inventory;

    private void Start()
    {
        _inventory = GetComponent<Inventory>();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.gKey.wasPressedThisFrame)
            TryThrow();
    }

    private void TryThrow()
    {
        if (_inventory == null || !_inventory.HasItem("Botella"))
        {
            Debug.Log("[BottleThrow] No tenés Botella en el inventario");
            return;
        }

        _inventory.RemoveItem("Botella");
        Debug.Log($"[BottleThrow] Botella lanzada! Restan: {_inventory.CountItem("Botella")}");

        CatStateMachineController cat = FindObjectOfType<CatStateMachineController>();
        if (cat != null)
        {
            cat.Flee();
            Debug.Log("[BottleThrow] Gato ahuyentado!");
        }
        else
        {
            Debug.Log("[BottleThrow] No hay gato en la escena");
        }
    }
}
