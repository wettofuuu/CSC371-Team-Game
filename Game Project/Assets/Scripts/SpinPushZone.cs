using UnityEngine;

public class SpinPushZone : MonoBehaviour
{
    public SpinPushBlock block;

    void Awake()
    {
        if (block == null)
            block = GetComponentInParent<SpinPushBlock>();
    }

    void OnTriggerStay(Collider other)
    {
        if (block == null) return;

        // Player must be in zone
        if (other.GetComponentInParent<Movement>() != null)
        {
            block.TryPushFromPlayer(other);
        }
    }
}
