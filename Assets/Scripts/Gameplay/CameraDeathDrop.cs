using UnityEngine;
using UnityEngine.Events;
using KittyTerror.Events;

public class CameraDeathDrop : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float dropHeight = 0.5f;
    [SerializeField] private float dropDuration = 1.2f;
    [SerializeField] private float tiltAngle = 35f;
    [SerializeField] private AnimationCurve dropCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private float fadeInDuration = 0.5f;

    [Header("Events")]
    [SerializeField] private UnityEvent onFadeCompleted;

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
            elapsed += Time.unscaledDeltaTime;
            float t = dropCurve.Evaluate(elapsed / dropDuration);
            camTransform.localPosition = Vector3.LerpUnclamped(_startLocalPosition, targetPosition, t);
            camTransform.localRotation = Quaternion.SlerpUnclamped(_startLocalRotation, targetRotation, t);
            yield return null;
        }

        camTransform.localPosition = targetPosition;
        camTransform.localRotation = targetRotation;

        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("player_death_impact"));

        if (fadeOverlay != null)
        {
            fadeOverlay.alpha = 0;
            fadeOverlay.gameObject.SetActive(true);

            float fadeElapsed = 0;

            while (fadeElapsed < fadeInDuration)
            {
                fadeElapsed += Time.unscaledDeltaTime;
                fadeOverlay.alpha = Mathf.Clamp01(fadeElapsed / fadeInDuration);
                yield return null;
            }

            fadeOverlay.alpha = 1;
        }

        onFadeCompleted?.Invoke();
    }
}
