using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseSceneController : MonoBehaviour
{
    [Tooltip("Name of your main menu scene")]
    public string mainMenuSceneName = "MainMenu";

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // just in case the game was paused
        SceneManager.LoadScene(mainMenuSceneName);
    }
}