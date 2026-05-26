using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PadlockUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button confirmButton;

    private NumberPadlock _padlock;

    private void Awake()
    {
        _padlock = GetComponent<NumberPadlock>();
        if (_padlock == null) return;

        _padlock.onPlayerEnter.AddListener(Show);
        _padlock.onPlayerExit.AddListener(Hide);

        if (confirmButton != null)
            confirmButton.onClick.AddListener(Submit);
    }

    private void Show()
    {
        if (panel != null) panel.SetActive(true);
        if (inputField != null)
        {
            inputField.text = "";
            inputField.ActivateInputField();
        }
    }

    private void Update()
    {
        if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Return))
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
