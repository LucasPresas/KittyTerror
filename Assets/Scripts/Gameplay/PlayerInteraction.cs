using UnityEngine;

using UnityEngine.InputSystem; 

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float interactDistance = 3.5f;
    [SerializeField] private LayerMask interactLayer;

    void Update()
    {
        
        var keyboard = Keyboard.current;
        
        
        if (keyboard == null) return;

        
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            
            ClockPuzzle clock = hit.collider.GetComponentInParent<ClockPuzzle>();

            if (clock != null)
            {
                
                if (keyboard.eKey.wasPressedThisFrame)
                {
                    clock.Interact();
                }
                
                
                if (keyboard.fKey.wasPressedThisFrame)
                {
                    clock.RotateHours();
                }
            }
        }
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * interactDistance);
    }
}