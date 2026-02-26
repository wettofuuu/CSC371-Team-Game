using UnityEngine;

public class SpinPuzzleZone : MonoBehaviour
{
    public SpinPuzzleMove block; 
    void Awake()
    {
        if (block == null)
            block = GetComponentInParent<SpinPuzzleMove>();
    }

    /*void OnTriggerStay(Collider other)
    {
        if (block == null) return;

        // Player must be in zone
        if (other.GetComponentInParent<Movement>() != null)
        {
            block.TryPushFromPlayer(other);
        }
    }*/
    void OnTriggerStay(Collider other)
    {
        //Debug.Log("TriggerStay by: " + other.name);

        if (block == null) Debug.Log("Block reference is null!");

        if (other.GetComponentInParent<Movement>() != null)
        {
            block.TryPushFromPlayer(other);
        }
    }
}
