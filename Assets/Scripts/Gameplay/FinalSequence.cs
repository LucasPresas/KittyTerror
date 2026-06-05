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
        public string role;
        public string name;
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

        // 6 — Credits
        if (credits != null)
        {
            foreach (CreditEntry entry in credits)
            {
                TextMeshProUGUI creditTmp = CreateText(canvasGO);
                creditTmp.text = $"<size=32>{entry.role}</size>\n<size=28>{entry.name}</size>";
                yield return new WaitForSeconds(creditDuration);
                Destroy(creditTmp.gameObject);
            }
        }

        // 7 — Play again button
        Button restartBtn = CreateButton(canvasGO, "JUGAR DE NUEVO");
        restartBtn.onClick.AddListener(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));

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

    private Button CreateButton(GameObject parent, string label)
    {
        GameObject go = new GameObject("RestartButton");
        go.transform.SetParent(parent.transform, false);

        Button btn = go.AddComponent<Button>();
        Image bg = go.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        GameObject labelGO = new GameObject("Label");
        labelGO.transform.SetParent(go.transform, false);
        TextMeshProUGUI tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 28;
        tmp.color = Color.white;

        RectTransform labelRt = labelGO.GetComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;

        RectTransform btnRt = go.GetComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.35f, 0.3f);
        btnRt.anchorMax = new Vector2(0.65f, 0.4f);
        btnRt.offsetMin = Vector2.zero;
        btnRt.offsetMax = Vector2.zero;

        return btn;
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
