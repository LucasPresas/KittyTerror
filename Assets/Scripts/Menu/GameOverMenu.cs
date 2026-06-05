using System.Collections;
using KittyTerror.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1f;

    private bool _isGameOver;

    private void Awake()
    {
        gameOverPanel.SetActive(false);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    private void OnEnable()
    {
        EventBus<GameOverEvent>.OnRaised += OnGameOver;
    }

    private void OnDisable()
    {
        EventBus<GameOverEvent>.OnRaised -= OnGameOver;
    }
    private void OnGameOver(GameOverEvent e)
    {
        Debug.Log($"Game Over: {e.Reason}");
        ShowGameOver();
    }
    public void ShowGameOver()
    {
        if (_isGameOver)
            return;

        _isGameOver = true;

        gameOverPanel.SetActive(true);
        Debug.Log(gameOverPanel.activeInHierarchy);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;

        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
