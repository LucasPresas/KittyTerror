using UnityEngine;

public class LockedDoor : MonoBehaviour, IInteractable
{
    [Header("Requerimiento")]
    [SerializeField] private string requiredItem = "Hacha";

    [Header("Golpes")]
    [SerializeField] private int maxHits = 3;

    [Header("Sprites")]
    [SerializeField] private Sprite[] damageStages;
    [SerializeField] private SpriteRenderer spriteRenderer;

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
        if (inv == null || !inv.HasItem(requiredItem)) return;

        _currentHits++;

        int stageIndex = _currentHits - 1;
        if (damageStages != null && stageIndex < damageStages.Length && spriteRenderer != null)
            spriteRenderer.sprite = damageStages[stageIndex];

        if (_currentHits >= maxHits)
        {
            _destroyed = true;
            Destroy(gameObject);
        }
    }
}
