using UnityEngine;

public class ClockPuzzle : MonoBehaviour, IInteractable
{
    [Header("Pivots de las Manecillas")]
    [SerializeField] private Transform hourPivot;
    [SerializeField] private Transform minutePivot;

    [Header("Configuración de Rotación")]
    [SerializeField] private float minuteStep = 45f;
    [SerializeField] private float hourStep = 30f; 

    [Header("UI Referencia")]
    [SerializeField] private GameObject visualHint; // Arrastrá acá el Canvas o el Texto

    public string GetInteractText() 
    {
        return "Ajustar Reloj\n[E] Minutos - [F] Horas";
    }

    // Esta función la va a llamar el Raycast del Player
    public void ToggleHint(bool state)
    {
        if (visualHint != null)
        {
            visualHint.SetActive(state);
        }
    }

    public void Interact()
    {
        RotateMinutes();
    }

    public void RotateHours()
    {
        if (hourPivot != null) hourPivot.Rotate(0, 0, -hourStep);
        CheckSolution();
    }

    private void RotateMinutes()
    {
        if (minutePivot != null) minutePivot.Rotate(0, 0, -minuteStep);
        CheckSolution();
    }

    private void CheckSolution() { /* Lógica de victoria aquí */ }
}