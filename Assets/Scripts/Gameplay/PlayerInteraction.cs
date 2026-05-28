using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private LayerMask interactLayer;

    private ClockPuzzle _lastClockLookedAt;
    private IInteractable _lastInteractable;

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            ClockPuzzle clock = hit.collider.GetComponentInParent<ClockPuzzle>();
            if (clock != null)
            {
                if (_lastClockLookedAt != clock)
                {
                    clock.ToggleHint(true);
                    _lastClockLookedAt = clock;
                }
                _lastInteractable = null;

                if (keyboard.eKey.wasPressedThisFrame) clock.Interact();
                if (keyboard.fKey.wasPressedThisFrame) clock.RotateHours();
                return;
            }

            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                _lastInteractable = interactable;
                ClearLastClock();

                bool isDoor = interactable is LockedDoor;
                if ((isDoor && mouse.leftButton.wasPressedThisFrame) ||
                    (!isDoor && keyboard.eKey.wasPressedThisFrame))
                    interactable.Interact();
                return;
            }

            ClearLastClock();
            _lastInteractable = null;
        }
        else
        {
            ClearLastClock();
            _lastInteractable = null;
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
