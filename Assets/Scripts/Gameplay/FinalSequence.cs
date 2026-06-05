using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class FinalSequence : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class CreditEntry
    {
        public string name;
        public string role;
    }

    [Header("Trigger")]
    [SerializeField] private string interactText = "Recordar";

    [Header("Diálogo")]
    [SerializeField] private string[] dialogueLines = new string[] { "Ahora lo recuerdo..." };
    [SerializeField] private float lineDuration = 3f;

    [Header("Audio")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip evilLaughClip;

    [Header("Imagen del malvado")]
    [SerializeField] private Texture evilImage;

    [Header("Texto final")]
    [SerializeField] private string finalMessage = "CONTINUARÁ...";

    [Header("Créditos")]
    [SerializeField] private CreditEntry[] credits;
    [SerializeField] private float creditDuration = 2f;

    [Header("Botón")]
    [SerializeField] private Button restartButton;

    [Header("Cámara")]
    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] private float shakeAngle = 10f;

    private Transform _camPivot;
    private Behaviour[] _playerScripts;
    private bool _used;
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
    }

    public string GetInteractText() => _used ? "" : interactText;

    public void Interact()
    {
        Debug.Log("[FinalSequence] Interact() llamado");
        if (_used) return;
        _used = true;
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

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

        // 2 — Music
        if (musicClip != null)
        {
            _audio.clip = musicClip;
            _audio.loop = true;
            _audio.Play();
        }

        // 3 — Dialogue
        GameObject canvasGO = CreateCanvas();
        TextMeshProUGUI tmp = CreateText(canvasGO);

        if (dialogueLines != null)
        {
            foreach (string line in dialogueLines)
            {
                tmp.text = line;
                yield return new WaitForSeconds(lineDuration);
            }
        }

        Destroy(tmp.gameObject);

        // 4 — Evil image
        if (evilImage != null)
        {
            RawImage img = CreateRawImage(canvasGO, evilImage);
            if (evilLaughClip != null)
                _audio.PlayOneShot(evilLaughClip);
            yield return new WaitForSeconds(2f);
            Destroy(img.gameObject);
        }

        // 5 — Final text
        if (finalMessage != null)
        {
            TextMeshProUGUI finalTmp = CreateText(canvasGO);
            finalTmp.text = finalMessage;
            finalTmp.fontSize = 48;
            yield return new WaitForSeconds(2f);
            Destroy(finalTmp.gameObject);
        }

        // 6 — Title + Credits
        if (credits != null)
        {
            TextMeshProUGUI titleTmp = null;
            int creditCount = credits.Length;

            for (int i = 0; i < creditCount; i++)
            {
                if (i == 0)
                {
                    titleTmp = CreateTitleText(canvasGO);
                    titleTmp.text = "KITTY TERROR";
                }

                CreditEntry entry = credits[i];
                TextMeshProUGUI creditTmp = CreateText(canvasGO);
                creditTmp.text = $"<size=32>{entry.name}</size>\n<size=28>{entry.role}</size>";
                yield return new WaitForSeconds(creditDuration);
                Destroy(creditTmp.gameObject);

                if (i == creditCount - 1)
                    Destroy(titleTmp.gameObject);
            }
        }

        // 7 — Show restart button
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() => SceneManager.LoadScene(0));
        }

        yield return null;
    }

    private GameObject CreateCanvas()
    {
        GameObject go = new GameObject("FinalCanvas");
        Canvas c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    private TextMeshProUGUI CreateText(GameObject parent)
    {
        GameObject go = new GameObject("FinalText");
        go.transform.SetParent(parent.transform, false);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 36;
        tmp.color = Color.white;
        tmp.richText = true;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return tmp;
    }

    private TextMeshProUGUI CreateTitleText(GameObject parent)
    {
        GameObject go = new GameObject("TitleText");
        go.transform.SetParent(parent.transform, false);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.alignment = TextAlignmentOptions.Top;
        tmp.fontSize = 52;
        tmp.color = Color.white;
        tmp.fontStyle = TMPro.FontStyles.Bold;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.7f);
        rt.anchorMax = new Vector2(1, 1);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return tmp;
    }

    private RawImage CreateRawImage(GameObject parent, Texture texture)
    {
        GameObject go = new GameObject("EvilImage");
        go.transform.SetParent(parent.transform, false);

        RawImage img = go.AddComponent<RawImage>();
        img.texture = texture;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return img;
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
