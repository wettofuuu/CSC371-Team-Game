using UnityEngine;

public class TeleportScene : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private string sceneName = "Level2Transition";
    [SerializeField] private LevelCompletePopup popup; // drag your popup in here

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // Only trigger for player
        if (other.GetComponentInParent<Movement>() == null) return;

        triggered = true;

        if (particleSystem != null)
            particleSystem.Play();

        // Tell the popup what scene "Continue" should load, then show it
        popup.SetNextScene(sceneName);
        popup.Show();
    }
}