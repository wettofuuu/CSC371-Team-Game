using System.Collections;
using UnityEngine;

public class TeleportScene : MonoBehaviour
{
    public string SceneName = "Level2Temp";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Teleport()); 
    }

    private IEnumerator Teleport(){
        if (ScreenFader.Instance != null){
            yield return ScreenFader.Instance.FadeToScene(SceneName);
         } else {
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
         }
    }
}
