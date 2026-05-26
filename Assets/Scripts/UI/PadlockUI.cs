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

    private void Awake()
    {
        _padlock = GetComponent<NumberPadlock>();
        Debug.Log($"[PadlockUI] Awake - padlock={( _padlock != null ? "found" : "NULL" )}, panel={panel?.name}, input={inputField?.name}");

        if (_padlock == null) return;

        _padlock.onPlayerEnter.AddListener(Show);
        _padlock.onPlayerExit.AddListener(Hide);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(Submit);
    }

    private void Show()
    {
        Debug.Log("[PadlockUI] Show");
        if (panel != null) panel.SetActive(true);
        if (inputField != null)
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    private void Update()
    {
        if (panel != null && panel.activeSelf && Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
            Submit();
    }

    private void Hide()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void Submit()
    {
        if (_padlock == null || inputField == null) return;

        _padlock.CheckCode(inputField.text);
    }
}
