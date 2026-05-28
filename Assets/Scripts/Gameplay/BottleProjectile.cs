using UnityEngine;
using UnityEngine.Events;
using KittyTerror.Gameplay;

public class BottleProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float hitboxRadius = 0.6f;

    public UnityEvent OnLaunched;
    public UnityEvent OnHit;

    private Vector3 _velocity;

    public void SetDirection(Vector3 dir)
    {
        _velocity = dir.normalized * speed;
    }

    private void Start()
    {
        CleanColliders();
        SetupRigidbody();
        IgnorePlayerCollision();

        Debug.Log($"[BottleProjectile] Lanzado desde {transform.position}, velocity={_velocity}");
        OnLaunched?.Invoke();
        Destroy(gameObject, lifetime);
    }

    private void CleanColliders()
    {
        Collider[] existing = GetComponents<Collider>();
        foreach (Collider c in existing)
            Destroy(c);

        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = hitboxRadius;
    }

    private void SetupRigidbody()
    {
        if (TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void IgnorePlayerCollision()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Collider myCollider = GetComponent<Collider>();
        if (myCollider == null) return;

        Collider[] playerColliders = player.GetComponentsInChildren<Collider>();
        foreach (Collider pc in playerColliders)
            Physics.IgnoreCollision(myCollider, pc);
    }

    private void Update()
    {
        _velocity.y += gravity * Time.deltaTime;
        transform.position += _velocity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[BottleProjectile] Impactó con: {other.name} (tag={other.tag})");

        if (other.TryGetComponent(out CatStateMachineController cat))
        {
            Debug.Log("[BottleProjectile] ¡Golpeó al gato! Llamando Flee()");
            cat.Flee();
        }

        OnHit?.Invoke();
        Destroy(gameObject);
    }
}
