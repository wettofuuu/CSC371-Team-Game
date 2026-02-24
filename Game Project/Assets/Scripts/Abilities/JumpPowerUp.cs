using UnityEngine;

public class JumpPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Movement m = other.GetComponentInParent<Movement>();
        if (m == null) return;

        m.EnableJump();
        gameObject.SetActive(false);
    }
}