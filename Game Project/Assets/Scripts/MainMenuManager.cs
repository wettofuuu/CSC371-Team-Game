using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Tooltip("Scene name to load when Play is pressed")]
    public string sceneToLoad = "TutorialScene";

    [Header("Main Menu Music")]
    [Tooltip("Drag the AudioSource that should play on the main menu (turn off Play On Awake on it).")]
    public AudioSource menuMusic;

    void Start()
    {
        // Ensure cursor is visible in the menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Make sure game isn't paused if coming from a paused scene
        Time.timeScale = 1f;

        // Stop anything that might still be playing (including persistent audio)
        foreach (var src in FindObjectsOfType<AudioSource>(true))
        {
            src.Stop();
        }

        // Now play ONLY the menu music
        if (menuMusic != null)
        {
            menuMusic.Play();
        }
        else
        {
            Debug.LogWarning("MainMenuController: menuMusic is not assigned.");
        }
    }

    // Called by Play button OnClick
    public void Play()
    {
        Time.timeScale = 1f;
        Movement.ResetLives();
        SceneManager.LoadScene(sceneToLoad);
    }

    // Called by Options button OnClick (does nothing)
    public void Options()
    {
        Debug.Log("Options pressed (not implemented)");
    }

    // Called by Quit button OnClick
    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}