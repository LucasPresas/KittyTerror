using UnityEngine;
using UnityEngine.InputSystem;
using TMPro; // No te olvides de esto para el texto

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración de Raycast")]
    [SerializeField] private float interactDistance = 3.5f;
    [SerializeField] private LayerMask interactLayer;

    [Header("Interfaz de Usuario")]
    [SerializeField] private GameObject hintObject; // El objeto del texto que creamos
    [SerializeField] private TextMeshProUGUI hintText; // El componente de texto

    private bool _hintVisible;

    void Awake()
    {
        if (hintObject != null && hintText == null)
        {
            hintText = hintObject.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (hintObject == null)
        {
            Debug.LogWarning($"[{nameof(PlayerInteraction)}] hintObject no está asignado en {name}. Se omite UI de interacción.", this);
        }

        SetHint(false);
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Tiramos el Raycast
        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            // Buscamos el puzzle
            ClockPuzzle clock = hit.collider.GetComponentInParent<ClockPuzzle>();

            if (clock != null)
            {
                // 1. SEÑALIZACIÓN: Activamos el cartel y pedimos el texto al objeto
                SetHint(true, clock.GetInteractText());

                // 2. INTERACCIÓN
                if (keyboard.eKey.wasPressedThisFrame)
                {
                    clock.Interact();
                }
                
                if (keyboard.fKey.wasPressedThisFrame)
                {
                    clock.RotateHours();
                }
            }
            else
            {
                // Si miramos algo de la capa Interactable pero no es el reloj
                SetHint(false);
            }
        }
        else
        {
            // Si no miramos nada interactuable, apagamos el cartel
            SetHint(false);
        }
    }

    private void SetHint(bool visible, string text = null)
    {
        if (hintObject != null && _hintVisible != visible)
        {
            hintObject.SetActive(visible);
            _hintVisible = visible;
        }

        if (visible && hintText != null && !string.IsNullOrEmpty(text))
        {
            hintText.text = text;
        }
    }
}
