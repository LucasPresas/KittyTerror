using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuración de Raycast")]
    [SerializeField] private float interactDistance = 5f; // Lo subí a 5 por las dudas
    [SerializeField] private LayerMask interactLayer;

    [Header("Interfaz de Usuario")]
    [SerializeField] private GameObject hintObject;
    [SerializeField] private TextMeshProUGUI hintText;

    private bool _hintVisible;

    void Awake()
    {
        if (hintObject != null && hintText == null)
        {
            hintText = hintObject.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (hintObject == null)
        {
            Debug.LogWarning($"<color=yellow>[PlayerInteraction]</color> hintObject no está asignado en {name}. Se omite UI.", this);
        }

        SetHint(false);
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 1. Dibujamos el rayo en la Scene para ver la distancia real
        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.cyan);

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 2. ¿El Raycast choca con ALGO?
        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            // 3. Imprimimos con qué objeto chocamos (útil para ver si la Layer está bien)
            Debug.Log($"<color=cyan>[Raycast]</color> Chocando con: {hit.collider.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)})");

            // 4. Buscamos el script
            ClockPuzzle clock = hit.collider.GetComponentInParent<ClockPuzzle>();

            if (clock != null)
            {
                // 5. ¡ENCONTRAMOS EL RELOJ!
                SetHint(true, clock.GetInteractText());

                if (keyboard.eKey.wasPressedThisFrame)
                {
                    Debug.Log("<color=green>[Input]</color> Tecla E presionada. Llamando a Interact()");
                    clock.Interact();
                }
                
                if (keyboard.fKey.wasPressedThisFrame)
                {
                    Debug.Log("<color=green>[Input]</color> Tecla F presionada. Llamando a RotateHours()");
                    clock.RotateHours();
                }
            }
            else
            {
                // El objeto está en la Layer correcta pero no tiene el script ClockPuzzle
                Debug.LogWarning($"<color=orange>[Puzzle]</color> El objeto {hit.collider.name} está en la Layer Interactable pero no tiene el script ClockPuzzle ni en él ni en sus padres.");
                SetHint(false);
            }
        }
        else
        {
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