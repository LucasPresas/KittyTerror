using KittyTerror.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ClickOnButton()
    {
        Debug.Log("[MenuManager] Raising click_button event"); 
        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("click_button"));
    }
    public void HoverOnButton()
    {
        Debug.Log("[MenuManager] Raising hover_button event");
        EventBus<AudioPlayEvent>.Raise(new AudioPlayEvent("hover_button"));
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game");
    }
}
