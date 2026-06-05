using UnityEngine;
using UnityEngine.Events;
using KittyTerror.Events;

public class BottleThrow : MonoBehaviour, IItemAction
{
    [SerializeField] private GameObject bottleInHand; // La botella visual en tu mano
    [SerializeField] private GameObject bottleProjectilePrefab; // El prefab de la botella que sale volando
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float spawnForwardDistance = 1.0f; // Distancia para que no choque contigo al spawnear

    public UnityEvent OnThrowAnimation;
    public UnityEvent OnThrow;

    private Inventory _inventory;
    private Camera _cam;

    private void Start()
    {
        _inventory = GetComponent<Inventory>();
        if (_inventory == null)
            _inventory = FindObjectOfType<Inventory>();

        _cam = Camera.main;

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
        OnThrow?.Invoke();

        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("bottle_throw"));
        LaunchProjectileLikeVideo();
    }

    private void LaunchProjectileLikeVideo()
    {
        if (bottleProjectilePrefab == null)
        {
            Debug.LogError("[BottleThrow] Falta asignar bottleProjectilePrefab en el inspector.");
            return;
        }

        // 1. Determinar desde dónde sale (preferiblemente la cámara)
        Transform origin = _cam != null ? _cam.transform : transform;
        Vector3 spawnPos = origin.position + origin.forward * spawnForwardDistance;
        
        // 2. Instanciar la botella suelta
        GameObject proj = Instantiate(bottleProjectilePrefab, spawnPos, origin.rotation);

        // 3. Activar físicas (igual que en el video)
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb == null) rb = proj.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = true;

        // 4. Agregar fuerza de impulso hacia adelante (como en el minuto 03:00)
        rb.AddForce(origin.forward * throwForce, ForceMode.Impulse);
    }
}