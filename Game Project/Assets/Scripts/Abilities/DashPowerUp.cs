using UnityEngine;

public class DashPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // tries to find Movement on self or parents (robust)
        Movement m = other.GetComponentInParent<Movement>();
        if (m == null) return;

        m.EnableDash();             // unlock dash
        gameObject.SetActive(false); // or Destroy(gameObject);
    }
}
