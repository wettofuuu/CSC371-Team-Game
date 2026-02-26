using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    [Header("Transition Sound")]
    public AudioSource transitionAudio;

    void Start()
    {
        // Stop any audio still playing from previous scene
        foreach (var src in FindObjectsOfType<AudioSource>(true))
        {
            src.Stop();
        }

        // Play transition sound
        if (transitionAudio != null)
        {
            transitionAudio.Play();
        }
        else
        {
            Debug.LogWarning("TransitionManager: No AudioSource assigned.");
        }
    }
}