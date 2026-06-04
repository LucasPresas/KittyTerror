using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private LayerMask interactLayer;

    private ClockPuzzle _lastClockLookedAt;
    private IInteractable _lastInteractable;
    private ItemEquip _itemEquip;

    private void Start()
    {
        _itemEquip = GetComponent<ItemEquip>();
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(ray, out hit, interactDistance, interactLayer);

        if (hitSomething)
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

                if (keyboard.eKey.wasPressedThisFrame)
                {
                    if (interactable is LockedDoor) return;
                    interactable.Interact();
                    return;
                }

                if (mouse.leftButton.wasPressedThisFrame && _itemEquip != null && !string.IsNullOrEmpty(_itemEquip.CurrentEquippedId))
                {
                    interactable.Interact();
                    return;
                }

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

        if (mouse.leftButton.wasPressedThisFrame && _itemEquip != null && !string.IsNullOrEmpty(_itemEquip.CurrentEquippedId))
        {
            _itemEquip.UseEquippedItem();
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
