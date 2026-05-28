using UnityEngine;
using UnityEngine.InputSystem;

public class BottleThrow : MonoBehaviour
{
    [SerializeField] private GameObject bottleProjectilePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float throwForce = 15f;

    private Inventory _inventory;

    private void Start()
    {
        _inventory = GetComponent<Inventory>();
        if (spawnPoint == null)
            spawnPoint = Camera.main?.transform ?? transform;
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

        if (bottleProjectilePrefab == null)
        {
            Debug.LogError("[BottleThrow] bottleProjectilePrefab no asignado");
            return;
        }

        _inventory.RemoveItem("Botella");

        GameObject bottle = Instantiate(bottleProjectilePrefab, spawnPoint.position, spawnPoint.rotation);
        bottle.transform.forward = spawnPoint.forward;

        Rigidbody rb = bottle.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(spawnPoint.forward * throwForce, ForceMode.VelocityChange);

        Destroy(bottle, 5f);
    }
}
