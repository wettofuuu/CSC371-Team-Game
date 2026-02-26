using UnityEngine;

public class LoseSceneAudioManager : MonoBehaviour
{
    [Header("Death Sound To Play")]
    public AudioSource deathSound;

    void Start()
    {
        // Stop every AudioSource in the entire scene (including persistent ones)
        AudioSource[] allAudio = FindObjectsOfType<AudioSource>(true);

        foreach (AudioSource src in allAudio)
        {
            src.Stop();
        }

        // Now play ONLY the death sound
        if (deathSound != null)
        {
            deathSound.Play();
        }
        else
        {
            Debug.LogWarning("LoseSceneAudioManager: deathSound not assigned.");
        }
    }
}