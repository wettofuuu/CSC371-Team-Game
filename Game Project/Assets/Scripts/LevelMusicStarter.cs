using UnityEngine;

public class LevelMusicStarter : MonoBehaviour
{
    void Start()
    {
        // Find the persistent BGM object
        DontDestroyMusic music = FindObjectOfType<DontDestroyMusic>();

        if (music != null)
        {
            AudioSource src = music.GetComponent<AudioSource>();

            if (src != null)
            {
                src.Stop();   // Reset it
                src.Play();   // Play it fresh
            }
        }
        else
        {
            Debug.LogWarning("LevelMusicStarter: No DontDestroyMusic found.");
        }
    }
}