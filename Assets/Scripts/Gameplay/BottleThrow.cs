using UnityEngine;
using UnityEngine.Events;

public class BottleThrow : MonoBehaviour, IItemAction
{
    [SerializeField] private GameObject bottleInHand;
<<<<<<< HEAD
    [SerializeField] private float throwForce = 15f;
    [SerializeField] private float destroyDelay = 3f;
=======
    [SerializeField] private GameObject bottleProjectilePrefab;
    [SerializeField] private Vector3 projectileRotation = new Vector3(-16.923f, 6.151f, 159.683f);
    [SerializeField] private float spawnForwardDistance = 1.5f;
    [SerializeField] private float spawnVerticalOffset = -0.2f;
>>>>>>> d3a7a9e (feat: botella arrojadiza con proyectil visual, arco y hitbox)

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

<<<<<<< HEAD
        LaunchBottle();

        CatStateMachineController cat = FindObjectOfType<CatStateMachineController>();
        if (cat != null)
            cat.Flee();
=======
        LaunchProjectile();
    }

    private void LaunchProjectile()
    {
        if (bottleProjectilePrefab == null)
        {
            Debug.LogError("[BottleThrow] bottleProjectilePrefab no asignado en el Inspector");
            return;
        }

        Transform origin = _cam != null ? _cam.transform : transform;

        Vector3 spawnPos = origin.position + origin.forward * spawnForwardDistance + origin.up * spawnVerticalOffset;
        GameObject proj = Instantiate(bottleProjectilePrefab, spawnPos, Quaternion.Euler(projectileRotation));

        if (!proj.TryGetComponent(out BottleProjectile bp))
        {
            bp = proj.AddComponent<BottleProjectile>();
        }
        bp.SetDirection(origin.forward);

        Debug.Log($"[BottleThrow] Proyectil lanzado desde {spawnPos}");
>>>>>>> d3a7a9e (feat: botella arrojadiza con proyectil visual, arco y hitbox)
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
