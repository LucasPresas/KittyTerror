using UnityEngine;
using KittyTerror.Gameplay;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BottleProjectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        CatStateMachineController cat = collision.collider.GetComponentInParent<CatStateMachineController>();
        if (cat != null)
        {
            cat.Flee();
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject, 0.1f);
        GetComponent<Renderer>().enabled = false;
    }
}
