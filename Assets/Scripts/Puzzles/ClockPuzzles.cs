using UnityEngine;

public class ClockPuzzle : MonoBehaviour, IInteractable
{
    [Header("Pivots de las Manecillas")]
    [SerializeField] private Transform hourPivot;
    [SerializeField] private Transform minutePivot;

    [Header("Configuración de Rotación")]
    [SerializeField] private float minuteStep = 45f;
    [SerializeField] private float hourStep = 30f; 

  public string GetInteractText() 
    {
        return "Ajustar Reloj\n[E] Minutos - [F] Horas";
    }    

    
    public void Interact()
    {
        RotateMinutes();
    }

    
    public void RotateHours()
    {
        hourPivot.Rotate(0, 0, -hourStep);
        CheckSolution();
    }

    private void RotateMinutes()
    {
        minutePivot.Rotate(0, 0, -minuteStep);
        CheckSolution();
    }

    private void CheckSolution()
    {
        
    }
}