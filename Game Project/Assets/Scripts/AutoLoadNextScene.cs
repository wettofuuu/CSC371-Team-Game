using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadNextScene : MonoBehaviour
{
    public string nextSceneName = "Level2Scene";
    public float delay = 1.5f;

    private bool triggered = false;

    void Start()
    {
        StartCoroutine(LoadAfterDelay());
    }

    IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSecondsRealtime(delay); // IMPORTANT (timescale-proof)

        if (triggered) yield break;
        triggered = true;

        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeToScene(nextSceneName);
        }
        else
        {
            // fallback if fader not present
            SceneManager.LoadScene(nextSceneName);
        }
    }
}