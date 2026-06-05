using UnityEngine;
using UnityEngine.Events;
using KittyTerror.Gameplay;
using KittyTerror.Events;

[RequireComponent(typeof(Rigidbody))]
public class BottleProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private GameObject breakVfx;

    public UnityEvent OnLaunched;
    public UnityEvent OnHit;

    private void Start()
    {
        Debug.Log("[BottleProjectile] Botella lanzada con físicas.");
        OnLaunched?.Invoke();
        
        Destroy(gameObject, lifetime);
    }

    // 1. Detecta choques físicos normales (paredes, piso, jugador)
    private void OnCollisionEnter(Collision collision)
    {
        ProcesarImpacto(collision.collider);
    }

    // 2. Detecta si atraviesa un Trigger (por si el gato está configurado como Trigger)
    private void OnTriggerEnter(Collider other)
    {
        ProcesarImpacto(other);
    }

    // Método centralizado que revisa a quién tocamos
    private void ProcesarImpacto(Collider col)
    {
        Debug.Log($"[BottleProjectile] Tocó con: {col.gameObject.name} (Tag: {col.gameObject.tag})");

        // Usamos GetComponentInParent por si golpeamos una pata, la cola o un collider hijo
        CatStateMachineController cat = col.GetComponentInParent<CatStateMachineController>();

        if (cat != null)
        {
            Debug.Log("[BottleProjectile] ¡Golpeó al gato! Llamando Flee()");
            EventBus<ThoughtEvent>.Raise(new ThoughtEvent("thought.bottle_throw"));
            cat.Flee();
        }

        if (breakVfx != null)
        {
            GameObject vfx = Instantiate(breakVfx, transform.position, Quaternion.identity);
            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            Destroy(vfx, ps != null ? ps.main.duration : 1f);
        }

        OnHit?.Invoke();
        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("bottle_break"));
        Destroy(gameObject);
    }
}