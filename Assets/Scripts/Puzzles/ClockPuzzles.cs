using UnityEngine;

public class ClockPuzzle : MonoBehaviour, IInteractable
{
    [Header("Pivots de las Manecillas")]
    [SerializeField] private Transform hourPivot;
    [SerializeField] private Transform minutePivot;

    [Header("Configuración de Rotación")]
    [SerializeField] private float minuteStep = 45f;
    [SerializeField] private float hourStep = 30f; 

    [Header("Condición de victoria")]
    [SerializeField] private float targetMinuteZ = 180f;
    [SerializeField] private float targetHourZ = 90f;
    [SerializeField] private float angleTolerance = 0.5f;

    [Header("UI Referencia")]
    [SerializeField] private GameObject visualHint; // Arrastrá acá el Canvas o el Texto

    [Header("Ajuste visual del hint")]
    [SerializeField] private bool keepHintFacingCamera = true;

    private Transform _hintRoot;
    private Camera _mainCamera;
    private bool _hasWon;

    private void Awake()
    {
        if (visualHint != null)
        {
            Canvas parentCanvas = visualHint.GetComponentInParent<Canvas>(true);
            _hintRoot = parentCanvas != null ? parentCanvas.transform : visualHint.transform;
            visualHint.SetActive(false);
        }

        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (!keepHintFacingCamera || visualHint == null || !visualHint.activeInHierarchy || _hintRoot == null)
            return;

        if (_mainCamera == null) _mainCamera = Camera.main;
        if (_mainCamera == null) return;

        Vector3 toCamera = _mainCamera.transform.position - _hintRoot.position;
        if (toCamera.sqrMagnitude > 0.0001f)
        {
            _hintRoot.rotation = Quaternion.LookRotation(toCamera, Vector3.up);
        }
    }

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

    private void CheckSolution()
    {
        if (_hasWon || minutePivot == null || hourPivot == null)
            return;

        float currentMinuteZ = NormalizeAngle(minutePivot.localEulerAngles.z);
        float currentHourZ = NormalizeAngle(hourPivot.localEulerAngles.z);

        bool minuteMatches = Mathf.Abs(Mathf.DeltaAngle(currentMinuteZ, targetMinuteZ)) <= angleTolerance;
        bool hourMatches = Mathf.Abs(Mathf.DeltaAngle(currentHourZ, targetHourZ)) <= angleTolerance;

        if (minuteMatches && hourMatches)
        {
            _hasWon = true;
            Debug.Log("ganaste", this);
        }
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }
}
