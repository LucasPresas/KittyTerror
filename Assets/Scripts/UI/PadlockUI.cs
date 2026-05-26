using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PadlockUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button confirmButton;

    private NumberPadlock _padlock;
    private float _hideTimer;

    private void Awake()
    {
        _padlock = GetComponent<NumberPadlock>();
        if (_padlock == null) return;

        _padlock.onPlayerEnter.AddListener(Show);
        _padlock.onPlayerExit.AddListener(QueueHide);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(Submit);
    }

    private void Show()
    {
        _hideTimer = 0f;
        if (panel != null) panel.SetActive(true);
        if (inputField != null)
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    private void Update()
    {
        if (_hideTimer > 0f)
        {
            _hideTimer -= Time.deltaTime;
            if (_hideTimer <= 0f && panel != null)
                panel.SetActive(false);
        }

        if (panel != null && panel.activeSelf && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            Submit();
    }

    private void QueueHide()
    {
        _hideTimer = 0.5f;
    }

    public void Submit()
    {
        if (_padlock == null || inputField == null) return;

        _padlock.CheckCode(inputField.text);
    }
}
