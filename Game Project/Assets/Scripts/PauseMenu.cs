
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

    public void Level1(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("TutorialScene");
    }

    public void Level2(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level2Scene");
    }

    public void Level3(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level3Scene");
    }

    public void Level4(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level4Design");
    }

    public void Level5(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("FinalBossScene");
    }
}
