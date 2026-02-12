using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadNextScene : MonoBehaviour
{
    public string nextSceneName = "Level2Scene";
    public float delay = 1.5f;

    void Start()
    {
        StartCoroutine(LoadAfterDelay());
    }

    IEnumerator LoadAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(nextSceneName);
    }
}
