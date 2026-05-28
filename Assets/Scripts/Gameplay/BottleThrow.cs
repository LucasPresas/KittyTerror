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

        Debug.Log($"[BottleThrow] Inicializado. Inventory: {_inventory != null}, SpawnPoint: {spawnPoint?.name}");
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            Debug.Log("[BottleThrow] Tecla G presionada");
            TryThrow();
        }
    }

    private void TryThrow()
    {
        if (_inventory == null)
        {
            Debug.Log("[BottleThrow] ERROR: Inventory component no encontrado en el GameObject");
            return;
        }

        Debug.Log($"[BottleThrow] Verificando Botella en inventario... Tiene: {_inventory.HasItem("Botella")}, Cantidad: {_inventory.CountItem("Botella")}");

        if (!_inventory.HasItem("Botella"))
        {
            Debug.Log("[BottleThrow] No tenés Botella en el inventario");
            return;
        }

        if (bottleProjectilePrefab == null)
        {
            Debug.LogError("[BottleThrow] bottleProjectilePrefab no asignado en el Inspector");
            return;
        }

        _inventory.RemoveItem("Botella");
        Debug.Log($"[BottleThrow] Botella consumida. Restan: {_inventory.CountItem("Botella")}");

        Vector3 spawnPos = spawnPoint.position;
        Quaternion spawnRot = spawnPoint.rotation;

        Debug.Log($"[BottleThrow] Instanciando proyectil en {spawnPos}, forward: {spawnPoint.forward}");
        GameObject bottle = Instantiate(bottleProjectilePrefab, spawnPos, spawnRot);
        bottle.transform.forward = spawnPoint.forward;

        Rigidbody rb = bottle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(spawnPoint.forward * throwForce, ForceMode.VelocityChange);
            Debug.Log($"[BottleThrow] Fuerza aplicada: {spawnPoint.forward * throwForce}");
        }
        else
        {
            Debug.LogError("[BottleThrow] El prefab de botella no tiene Rigidbody");
        }

        Destroy(bottle, 5f);
    }
}
