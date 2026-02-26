using System.Collections;
using UnityEngine;

public class TeleportScene : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    public string SceneName = "Level2Transition";

    private bool teleporting = false;

    private void OnTriggerEnter(Collider other)
    {
        if (teleporting) return;

        // Only teleport player
        if (other.GetComponentInParent<Movement>() == null) return;

        teleporting = true;

        if (particleSystem != null)
            particleSystem.Play();

        StartCoroutine(Teleport());
    }

    private IEnumerator Teleport()
    {
        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeToScene(SceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
    }
}