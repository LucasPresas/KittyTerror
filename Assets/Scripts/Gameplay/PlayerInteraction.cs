using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private LayerMask interactLayer;

    private ClockPuzzle _lastClockLookedAt;

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
                // Si es un reloj nuevo, prendemos el texto
                if (_lastClockLookedAt != clock)
                {
                    clock.ToggleHint(true);
                    _lastClockLookedAt = clock;
                }

                // Interacciones
                if (keyboard.eKey.wasPressedThisFrame) clock.Interact();
                if (keyboard.fKey.wasPressedThisFrame) clock.RotateHours();
            }
            else
            {
                // Si miramos algo de la capa interactable que NO es un reloj
                ClearLastClock();
            }
        }
        else
        {
            // Si el Raycast no choca con nada de la capa interactable
            ClearLastClock();
        }
    }

    private void ClearLastClock()
    {
        if (_lastClockLookedAt != null)
        {
            _lastClockLookedAt.ToggleHint(false);
            _lastClockLookedAt = null;
        }
    }
}