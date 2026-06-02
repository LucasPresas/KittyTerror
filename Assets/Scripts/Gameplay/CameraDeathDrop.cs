using UnityEngine;
using KittyTerror.Events;

public class CameraDeathDrop : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float dropHeight = 0.5f;
    [SerializeField] private float dropDuration = 1.2f;
    [SerializeField] private float tiltAngle = 35f;
    [SerializeField] private AnimationCurve dropCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 _startLocalPosition;
    private Quaternion _startLocalRotation;
    private bool _isDropping;

    private void OnEnable()
    {
        EventBus<GameOverEvent>.OnRaised += OnGameOver;
    }

    private void OnDisable()
    {
        EventBus<GameOverEvent>.OnRaised -= OnGameOver;
    }

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera != null)
        {
            _startLocalPosition = playerCamera.transform.localPosition;
            _startLocalRotation = playerCamera.transform.localRotation;
        }
    }

    private void OnGameOver(GameOverEvent e)
    {
        if (_isDropping || playerCamera == null) return;
        _isDropping = true;
        StartCoroutine(DropRoutine());
    }

    private System.Collections.IEnumerator DropRoutine()
    {
        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("player_death_drop"));

        Transform camTransform = playerCamera.transform;
        Vector3 targetPosition = _startLocalPosition + Vector3.down * dropHeight;
        Quaternion targetRotation = _startLocalRotation * Quaternion.Euler(tiltAngle, 0, 0);

        float elapsed = 0;

        while (elapsed < dropDuration)
        {
            elapsed += Time.deltaTime;
            float t = dropCurve.Evaluate(elapsed / dropDuration);
            camTransform.localPosition = Vector3.LerpUnclamped(_startLocalPosition, targetPosition, t);
            camTransform.localRotation = Quaternion.SlerpUnclamped(_startLocalRotation, targetRotation, t);
            yield return null;
        }

        camTransform.localPosition = targetPosition;
        camTransform.localRotation = targetRotation;

        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("player_death_impact"));
    }
}
