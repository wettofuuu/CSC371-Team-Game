using UnityEngine;

public class SpinPushZone : MonoBehaviour
{
    private IPushableBlock block;

    void Awake()
    {
        block = GetComponentInParent<IPushableBlock>();
    }

    void OnTriggerStay(Collider other)
    {
        if (block == null) return;

        if (other.GetComponentInParent<Movement>() != null)
        {
            block.TryPushFromPlayer(other);
        }
    }
}