using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ItemEquip : MonoBehaviour
{
    [System.Serializable]
    public class HotkeySlot
    {
        public int hotkey;
        public string itemId;
        public GameObject visualInHand;
    }

    [SerializeField] private HotkeySlot[] slots;

    public UnityEvent<string> OnItemUsed;
    public UnityEvent<string> OnEquip;
    public UnityEvent<string> OnUnequip;

    public string CurrentEquippedId { get; private set; } = "";

    private Inventory _inventory;

    private void Start()
    {
        _inventory = GetComponent<Inventory>();
        if (_inventory == null)
            _inventory = FindObjectOfType<Inventory>();

        if (_inventory != null)
            _inventory.OnItemRemoved.AddListener(OnItemRemoved);

        AutoWireItemActions();
    }

    private void OnItemRemoved(string itemId)
    {
        if (CurrentEquippedId == itemId)
            Unequip();
    }

    private void AutoWireItemActions()
    {
        var actions = GetComponents<IItemAction>();
        foreach (var action in actions)
        {
            OnItemUsed.AddListener(action.OnItemUsed);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        for (int i = 0; i < slots.Length; i++)
        {
            Key key = Key.Digit1 + i;
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                TryEquip(slots[i]);
                return;
            }
        }
    }

    private void TryEquip(HotkeySlot slot)
    {
        if (_inventory == null) return;
        if (!_inventory.HasItem(slot.itemId)) return;

        if (CurrentEquippedId == slot.itemId)
        {
            Unequip();
            return;
        }

        Unequip();

        CurrentEquippedId = slot.itemId;
        if (slot.visualInHand != null)
        {
            SetPhysicsEnabled(slot.visualInHand, false);
            slot.visualInHand.SetActive(true);
        }

        OnEquip?.Invoke(slot.itemId);
    }

    public void Unequip()
    {
        if (string.IsNullOrEmpty(CurrentEquippedId)) return;

        foreach (var s in slots)
        {
            if (s.itemId == CurrentEquippedId && s.visualInHand != null)
                s.visualInHand.SetActive(false);
        }

        string previous = CurrentEquippedId;
        CurrentEquippedId = "";
        OnUnequip?.Invoke(previous);
    }

    public static void SetPhysicsEnabled(GameObject obj, bool enabled)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = enabled;
            rb.isKinematic = !enabled;
        }
    }

    public void UseEquippedItem()
    {
        if (string.IsNullOrEmpty(CurrentEquippedId)) return;
        OnItemUsed?.Invoke(CurrentEquippedId);
    }
}
