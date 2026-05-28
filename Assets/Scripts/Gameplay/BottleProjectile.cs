using UnityEngine;
using KittyTerror.Gameplay;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BottleProjectile : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"[BottleProjectile] Instanciado en {transform.position}, forward: {transform.forward}");

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            Debug.LogError("[BottleProjectile] No tiene Rigidbody");
        else
            Debug.Log($"[BottleProjectile] Rigidbody: mass={rb.mass}, drag={rb.drag}, velocity={rb.linearVelocity}");

        Collider col = GetComponent<Collider>();
        if (col == null)
            Debug.LogError("[BottleProjectile] No tiene Collider");
        else
            Debug.Log($"[BottleProjectile] Collider: {col.GetType().Name}, isTrigger={col.isTrigger}");
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[BottleProjectile] Colisionó con: {collision.collider.name} (tag: {collision.collider.tag}, layer: {LayerMask.LayerToName(collision.collider.gameObject.layer)})");

        CatStateMachineController cat = collision.collider.GetComponentInParent<CatStateMachineController>();
        if (cat != null)
        {
            Debug.Log("[BottleProjectile] GATO ENCONTRADO! Llamando cat.Flee()");
            cat.Flee();
            Destroy(gameObject);
            return;
        }

        Debug.Log("[BottleProjectile] No era el gato, destruyendo botella");
        Destroy(gameObject, 0.1f);
    }
}
