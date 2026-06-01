using UnityEngine;
using UnityEngine.Events;
using KittyTerror.Events;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Requerimiento")]
    [SerializeField] private string requiredItem = "Hacha";

    [Header("Golpes")]
    [SerializeField] private int maxHits = 3;

    [Header("Sprites")]
    [SerializeField] private Sprite[] damageStages;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Events")]
    public UnityEvent OnHit;

    private int _currentHits;
    private bool _destroyed;

    public string GetInteractText()
    {
        if (_destroyed) return "";
        int remaining = maxHits - _currentHits;
        return remaining > 0 ? $"Golpear puerta ({remaining} golpes)" : "";
    }

    public void Interact()
    {
        if (_destroyed) return;

        Inventory inv = FindObjectOfType<Inventory>();
        if (inv == null || !inv.HasItem(requiredItem))
        {
            Debug.Log($"[PuertaHacha] Necesitas {requiredItem} para golpear la puerta");
            return;
        }

        _currentHits++;
        int remaining = maxHits - _currentHits;

        Debug.Log($"[PuertaHacha] {requiredItem} golpea PuertaHacha — sufrió {_currentHits} de daño, le quedan {remaining} de vida");

        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("door_hit"));
        OnHit?.Invoke();

        int stageIndex = _currentHits - 1;
        if (damageStages != null && stageIndex < damageStages.Length && spriteRenderer != null)
            spriteRenderer.sprite = damageStages[stageIndex];

        if (_currentHits >= maxHits)
        {
            Debug.Log("[PuertaHacha] Puerta destruida");
            EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("door_break"));
            _destroyed = true;
            Destroy(gameObject);
        }
    }
}
