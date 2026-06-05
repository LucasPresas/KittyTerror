using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FinalSequence : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class DialogueStep
    {
        public string text;
        public float displayTime = 3f;
    }

    [Header("Trigger")]
    [SerializeField] private string interactText = "Recordar";

    [Header("Cámara")]
    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] private float shakeAngle = 10f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip evilLaughClip;

    [Header("Diálogo")]
    [SerializeField] private DialogueStep[] dialogueSteps;

    [Header("UI")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private CanvasGroup dialogueGroup;
    [SerializeField] private RawImage evilImage;
    [SerializeField] private CanvasGroup finalGroup;
    [SerializeField] private TMP_Text finalText;
    [SerializeField] private string finalMessage = "CONTINUARÁ...";
    [SerializeField] private string menuSceneName = "MainScene";

    private Transform _camPivot;
    private Behaviour[] _playerScripts;
    private bool _used;

    private void Awake()
    {
        if (canvas != null) canvas.gameObject.SetActive(true);
        if (dialogueGroup != null) dialogueGroup.alpha = 0;
        if (finalGroup != null) finalGroup.alpha = 0;
        if (evilImage != null) evilImage.gameObject.SetActive(false);
    }

    public string GetInteractText() => _used ? "" : interactText;

    public void Interact()
    {
        if (_used) return;
        _used = true;
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerScripts = player.GetComponents<Behaviour>();
            _camPivot = player.transform.Find("CameraPivot");
        }

        SetPlayerEnabled(false);

        // 1 — Camera shake
        if (_camPivot != null)
            yield return StartCoroutine(ShakeCamera());

        // 2 — Change music
        if (audioSource != null && musicClip != null)
        {
            audioSource.clip = musicClip;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 3 — Dialogue
        if (dialogueText != null && dialogueGroup != null)
        {
            foreach (var step in dialogueSteps)
            {
                dialogueText.text = step.text;
                yield return StartCoroutine(FadeGroup(dialogueGroup, 0, 1, 0.3f));
                yield return new WaitForSeconds(step.displayTime);
                yield return StartCoroutine(FadeGroup(dialogueGroup, 1, 0, 0.3f));
            }
        }

        // 4 — Evil image + laugh
        if (evilImage != null)
        {
            evilImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeImage(evilImage, 0, 1, 0.2f));
        }

        if (audioSource != null && evilLaughClip != null)
            audioSource.PlayOneShot(evilLaughClip);

        yield return new WaitForSeconds(2f);

        // 5 — Final screen
        if (finalGroup != null)
        {
            if (finalText != null) finalText.text = finalMessage;
            yield return StartCoroutine(FadeGroup(finalGroup, 0, 1, 0.5f));
        }

        // Wait for click to return to menu
        while (!Input.GetMouseButtonDown(0))
            yield return null;

        SceneManager.LoadScene(menuSceneName);
    }

    private IEnumerator ShakeCamera()
    {
        Quaternion startRot = _camPivot.localRotation;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float t = elapsed / shakeDuration;
            float sway = Mathf.Sin(t * Mathf.PI * 3f) * shakeAngle * (1f - t);
            _camPivot.localRotation = startRot * Quaternion.Euler(8f, sway, 4f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _camPivot.localRotation = startRot;
    }

    private IEnumerator FadeGroup(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        group.alpha = to;
    }

    private IEnumerator FadeImage(RawImage image, float from, float to, float duration)
    {
        float elapsed = 0f;
        Color c = image.color;
        while (elapsed < duration)
        {
            c.a = Mathf.Lerp(from, to, elapsed / duration);
            image.color = c;
            elapsed += Time.deltaTime;
            yield return null;
        }
        c.a = to;
        image.color = c;
    }

    private void SetPlayerEnabled(bool enabled)
    {
        if (_playerScripts == null) return;
        foreach (Behaviour script in _playerScripts)
        {
            string name = script.GetType().Name;
            if (name == "FirstPersonStateMachineController" || name == "PlayerInteraction")
                script.enabled = enabled;
        }
    }
}
