using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using KittyTerror.Events;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject quitConfirmationPanel;
    [SerializeField] private bool startPaused = false;
    [SerializeField] private GameObject settingsMenu;

    private InputAction _pauseAction;
    private bool _isPaused;

    private void Awake()
    {
        _pauseAction = new InputAction("Pause", InputActionType.Button);
        _pauseAction.AddBinding("<Keyboard>/escape");
        _pauseAction.AddBinding("<Keyboard>/p");
        _pauseAction.AddBinding("<Gamepad>/start");

        if (container == null)
        {
            Debug.LogWarning($"[{nameof(PauseMenu)}] container no está asignado en {name}.", this);
            return;
        }

        AutoWireContinueButtons();

        SetPaused(startPaused);
    }

    private void OnEnable()
    {
        _pauseAction?.Enable();
    }

    private void OnDisable()
    {
        _pauseAction?.Disable();
    }

    private void OnDestroy()
    {
        _pauseAction?.Dispose();
    }

    void Update()
    {
        if (_pauseAction != null && _pauseAction.WasPressedThisFrame())
        {
            TogglePause();
        }
    }

    public void Pause()
    {
        SetPaused(true);
    }

    public void Unpause()
    {
        SetPaused(false);
    }

    public void BackToMenu()
    {
        SetPaused(false);
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowSettingsMenu()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(true);
        }
    }

    public void ShowQuitConfirmation()
    {
        if (quitConfirmationPanel != null)
        {
            quitConfirmationPanel.SetActive(true);
        }
    }

    public void ConfirmQuit()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    public void CancelQuit()
    {
        if (quitConfirmationPanel != null)
        {
            quitConfirmationPanel.SetActive(false);
        }
    }

    private void TogglePause()
    {
        SetPaused(!_isPaused);
    }

    private void SetPaused(bool paused)
    {
        _isPaused = paused;

        if (container != null)
        {
            container.SetActive(paused);
        }

        if (!paused && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Time.timeScale = paused ? 0f : 1f;
        Cursor.visible = paused;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void AutoWireContinueButtons()
    {
        if (container == null)
        {
            return;
        }

        Button[] buttons = container.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            if (button == null)
            {
                continue;
            }

            string lowerName = button.name.ToLowerInvariant();

            if (lowerName.Contains("continue") || lowerName.Contains("resume"))
            {
                button.onClick.AddListener(Unpause);
            }
            if (lowerName.Contains("settings"))
            {
                button.onClick.AddListener(ShowSettingsMenu);
            }
            if (lowerName.Contains("menu"))
            {
                button.onClick.AddListener(BackToMenu);
            }
            if (lowerName.Contains("quit") || lowerName.Contains("exit"))
            {
                button.onClick.AddListener(ShowQuitConfirmation);
            }
        }
    }

}
