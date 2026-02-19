using UnityEngine;

public class JumpPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // If your player has tag "Player", you can also check that:
        // if (!other.CompareTag("Player")) return;

        Movement m = other.GetComponent<Movement>();
        if (m == null) return;

        m.EnableJump();          // unlock jump
        gameObject.SetActive(false); // or Destroy(gameObject);
    }
}
