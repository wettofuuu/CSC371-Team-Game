
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;   

public class PauseMenu : MonoBehaviour
{
    public GameObject Container;
    private bool isPaused;

    void Start()
    {
        if (Container != null) Container.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;

        if (Container != null) Container.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void ResumeButton()
    {
        isPaused = false;
        if (Container != null) Container.SetActive(false);
        Time.timeScale = 1f;
    }

    public void MainMenuButton()
    {
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}
